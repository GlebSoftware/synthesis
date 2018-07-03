﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EditorsLibrary;
using JointResolver.ControlGUI;
using OGLViewer;

public delegate bool ValidationAction(RigidNode_Base baseNode, out string message);

[Guid("ec18f8d4-c13e-4c86-8148-7414efb6e1e2")]
public partial class SynthesisGUI : Form
{
    //public event Action ExportFinished;
    //public void OnExportFinished()
    //{
    //    ExportFinished.Invoke();
    //}

    public class RuntimeMeta
    {
        public bool UseSettingsDir;
        public string ActiveDir;
        public string ActiveRobotName;
        public bool OpenSynthesis;
        public string FieldName;

        public static RuntimeMeta CreateRuntimeMeta()
        {
            return new RuntimeMeta
            {
                UseSettingsDir = true,
                ActiveDir = null,
                ActiveRobotName = null,
                OpenSynthesis = false,
                FieldName = null
            };
        }
    }

    public RuntimeMeta RMeta = RuntimeMeta.CreateRuntimeMeta();

    public static SynthesisGUI Instance;

    public static PluginSettingsForm.PluginSettingsValues PluginSettings;

    public Form BXDAViewerPaneForm = new Form
    {
        FormBorderStyle = FormBorderStyle.None
    };
    public Form JointPaneForm = new Form
    {
        FormBorderStyle = FormBorderStyle.None
    };

    public RigidNode_Base SkeletonBase = null;
    public List<BXDAMesh> Meshes = null;
    public bool MeshesAreColored = false;
    public float TotalMass = 120;

    private SkeletonExporterForm skeletonExporter;
    private LiteExporterForm liteExporter;

    static SynthesisGUI()
    {
    }

    public SynthesisGUI(bool MakeOwners = false)
    {
        InitializeComponent();

        Instance = this;

        bxdaEditorPane1.Units = "lbs";
        BXDAViewerPaneForm.Controls.Add(bxdaEditorPane1);
        if (MakeOwners) BXDAViewerPaneForm.Owner = this;
        BXDAViewerPaneForm.FormClosing += Generic_FormClosing;

        JointPaneForm.Controls.Add(jointEditorPane1);
        if (MakeOwners) JointPaneForm.Owner = this;
        JointPaneForm.FormClosing += Generic_FormClosing;


        RigidNode_Base.NODE_FACTORY = delegate (Guid guid)
        {
            return new OGL_RigidNode(guid);
        };

        settingsExporter.Click += SettingsExporter_OnClick;

        Shown += SynthesisGUI_Shown;

        FormClosing += new FormClosingEventHandler(delegate (object sender, FormClosingEventArgs e)
        {
            if (SkeletonBase != null && !WarnUnsaved()) e.Cancel = true;
            InventorManager.ReleaseInventor();
        });

        jointEditorPane1.ModifiedJoint += delegate (List<RigidNode_Base> nodes)
        {

            if (nodes == null || nodes.Count == 0) return;

            foreach (RigidNode_Base node in nodes)
            {
                if (node.GetSkeletalJoint() != null && node.GetSkeletalJoint().cDriver != null &&
                    node.GetSkeletalJoint().cDriver.GetInfo<WheelDriverMeta>() != null &&
                    node.GetSkeletalJoint().cDriver.GetInfo<WheelDriverMeta>().radius == 0 &&
                    node is OGL_RigidNode)
                {
                    (node as OGL_RigidNode).GetWheelInfo(out float radius, out float width, out BXDVector3 center);

                    WheelDriverMeta wheelDriver = node.GetSkeletalJoint().cDriver.GetInfo<WheelDriverMeta>();
                    wheelDriver.center = center;
                    wheelDriver.radius = radius;
                    wheelDriver.width = width;
                    node.GetSkeletalJoint().cDriver.AddInfo(wheelDriver);

                }
            }
        };

        jointEditorPane1.SelectedJoint += bxdaEditorPane1.SelectJoints;

        bxdaEditorPane1.NodeSelected += (BXDAMesh mesh) =>
            {
                List<RigidNode_Base> nodes = new List<RigidNode_Base>();
                SkeletonBase.ListAllNodes(nodes);

                jointEditorPane1.AddSelection(nodes[Meshes.IndexOf(mesh)], true);
            };
    }

    private void Generic_FormClosing(object sender, FormClosingEventArgs e)
    {
        foreach (Form f in OwnedForms)
        {
            if(f.Visible)
                f.Close();
        }
        Close();
    }

    private void SynthesisGUI_Shown(object sender, EventArgs e)
    {
        Hide();
        BXDAViewerPaneForm.Show();
        JointPaneForm.Show();
    }

    public void SetNew()
    {
        if (SkeletonBase == null || !WarnUnsaved()) return;

        SkeletonBase = null;
        Meshes = null;
        ReloadPanels();
    }

    /// <summary>
    /// Build the node tree of the robot from Inventor
    /// </summary>
    public bool BuildRobotSkeleton(bool warnUnsaved = false)
    {
        if (SkeletonBase != null && warnUnsaved && !WarnUnsaved()) return false;

        try
        {
            var exporterThread = new Thread(() =>
            {
                skeletonExporter = new SkeletonExporterForm();
                skeletonExporter.ShowDialog();
            });

            exporterThread.SetApartmentState(ApartmentState.STA);
            exporterThread.Start();

            exporterThread.Join();

            GC.Collect();
        }
        catch (InvalidComObjectException)
        {
        }
        catch (TaskCanceledException)
        {
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }

        if (SkeletonBase == null)
            return false; // Skeleton export failed

        return true;
    }

    /// <summary>
    /// Export a robot from Inventor
    /// </summary>
    public bool ExportMeshes()
    {
        try
        {
            var exporterThread = new Thread(() =>
            {
                if (SkeletonBase == null)
                {
                    skeletonExporter = new SkeletonExporterForm();
                    skeletonExporter.ShowDialog();
                }

                if (SkeletonBase != null)
                {
#if LITEMODE
                    liteExporter = new LiteExporterForm();
                    liteExporter.ShowDialog(); // Remove node building
#else
                    exporter = new ExporterForm(PluginSettings);
                    exporter.ShowDialog();
#endif
                }
            });

            exporterThread.SetApartmentState(ApartmentState.STA);
            exporterThread.Start();

            exporterThread.Join();

            GC.Collect();

            MeshesAreColored = PluginSettings.GeneralUseFancyColors;
        }
        catch (InvalidComObjectException)
        {
        }
        catch (TaskCanceledException)
        {
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
            return false;
        }
        
        if (Meshes == null)
            return false; // Meshes were not exported
        
        if (liteExporter.DialogResult != DialogResult.OK)
            return false; // Exporter was canceled

        // Export was successful
        List<RigidNode_Base> nodes = SkeletonBase.ListAllNodes();
        for (int i = 0; i < Meshes.Count; i++)
        {
            ((OGL_RigidNode)nodes[i]).loadMeshes(Meshes[i]);
        }
            
        return true;
    }

    /// <summary>
    /// Open a previously exported robot. 
    /// </summary>
    /// <param name="validate">If it is not null, this will validate the open inventor assembly.</param>
    public void OpenExisting()
    {
        if (SkeletonBase != null && !WarnUnsaved()) return;

        string dirPath = OpenFolderPath();

        if (dirPath == null) return;

        try
        {
            List<RigidNode_Base> nodes = new List<RigidNode_Base>();
            SkeletonBase = BXDJSkeleton.ReadSkeleton(dirPath + "\\skeleton.bxdj");

            SkeletonBase.ListAllNodes(nodes);

            Meshes = new List<BXDAMesh>();

            foreach (RigidNode_Base n in nodes)
            {
                BXDAMesh mesh = new BXDAMesh();
                mesh.ReadFromFile(dirPath + "\\" + n.ModelFileName);

                if (!n.GUID.Equals(mesh.GUID))
                {
                    MessageBox.Show(n.ModelFileName + " has been modified.", "Could not load mesh.");
                    return;
                }

                Meshes.Add(mesh);
            }
            for (int i = 0; i < Meshes.Count; i++)
            {
                ((OGL_RigidNode)nodes[i]).loadMeshes(Meshes[i]);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }


        ReloadPanels();
    }

    /// <summary>
    /// Open a previously exported robot. 
    /// </summary>
    /// <param name="validate">If it is not null, this will validate the open inventor assembly.</param>
    public bool OpenExisting(ValidationAction validate, bool warnUnsaved = false)
    {

        if (SkeletonBase != null && warnUnsaved && !WarnUnsaved()) return false;

        string dirPath = OpenFolderPath();

        if (dirPath == null) return false;

        try
        {
            List<RigidNode_Base> nodes = new List<RigidNode_Base>();
            SkeletonBase = BXDJSkeleton.ReadSkeleton(dirPath + "\\skeleton.bxdj");

            if (validate != null)
            {
                if (!validate(SkeletonBase, out string message))
                {
                    while (true)
                    {
                        DialogResult result = MessageBox.Show(message, "Assembly Validation", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        if (result == DialogResult.Retry)
                            continue;
                        if (result == DialogResult.Abort)
                        {
                            return false;
                        }
                        break;
                    }
                }
                #region DEBUG
#if DEBUG
                else
                {
                    MessageBox.Show(message);
                }
#endif 
                #endregion
            }

            SkeletonBase.ListAllNodes(nodes);

            Meshes = new List<BXDAMesh>();

            foreach (RigidNode_Base n in nodes)
            {
                BXDAMesh mesh = new BXDAMesh();
                mesh.ReadFromFile(dirPath + "\\" + n.ModelFileName);

                if (!n.GUID.Equals(mesh.GUID))
                {
                    MessageBox.Show(n.ModelFileName + " has been modified.", "Could not load mesh.");
                    return false;
                }

                Meshes.Add(mesh);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString());
        }

        RMeta.UseSettingsDir = false;
        RMeta.ActiveDir = dirPath;
        RMeta.ActiveRobotName = dirPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries).Last();

        ReloadPanels();
        return true;
    }

    /// <summary>
    /// Prompts the user for the name of the robot, as well as other information.
    /// </summary>
    /// <returns>True if user pressed okay, false if they pressed cancel</returns>
    public bool PromptSaveSettings(bool allowOpeningSynthesis, bool isFinal)
    {
        if (SaveRobotForm.Prompt(RMeta.ActiveRobotName, allowOpeningSynthesis, isFinal, out string robotName, out bool colors, out bool openSynthesis, out string field) == DialogResult.OK)
        {
            RMeta.UseSettingsDir = true;
            RMeta.ActiveDir = null;
            RMeta.ActiveRobotName = robotName;
            RMeta.OpenSynthesis = openSynthesis;
            RMeta.FieldName = field;

            PluginSettings.GeneralUseFancyColors = colors;
            PluginSettings.OnSettingsChanged(PluginSettings.InventorChildColor, PluginSettings.GeneralUseFancyColors, PluginSettings.GeneralSaveLocation);

            return true;
        }
        return false;
    }

    /// <summary>
    /// Saves the robot to the directory it was loaded from or the default directory
    /// </summary>
    /// <returns></returns>
    public bool RobotSave(bool silent = true)
    {
        try
        {
            // If robot has not been named, prompt user for information
            if (RMeta.ActiveRobotName == null)
                if (!PromptSaveSettings(false, false))
                    return false;

            if (!Directory.Exists(PluginSettings.GeneralSaveLocation + "\\" + RMeta.ActiveRobotName))
                Directory.CreateDirectory(PluginSettings.GeneralSaveLocation + "\\" + RMeta.ActiveRobotName);

            if (Meshes == null || MeshesAreColored != PluginSettings.GeneralUseFancyColors) // Re-export if color settings changed
                ExportMeshes();

            BXDJSkeleton.SetupFileNames(SkeletonBase);
            BXDJSkeleton.WriteSkeleton((RMeta.UseSettingsDir && RMeta.ActiveDir != null) ? RMeta.ActiveDir : PluginSettings.GeneralSaveLocation + "\\" + RMeta.ActiveRobotName + "\\skeleton.bxdj", SkeletonBase);

            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].WriteToFile((RMeta.UseSettingsDir && RMeta.ActiveDir != null) ? RMeta.ActiveDir : PluginSettings.GeneralSaveLocation + "\\" + RMeta.ActiveRobotName + "\\node_" + i + ".bxda");
            }

            if(!silent)
                MessageBox.Show("Saved");

            return true;
        }
        catch (Exception e)
        {
            //TODO: Create a form that displays a simple error message with an option to expand it and view the exception info
            MessageBox.Show("Error saving robot: " + e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Saves the robot to the currently set robot directory.
    /// </summary>
    /// <param name="robotName"></param>
    public bool RobotSaveAs()
    {
        if (PromptSaveSettings(false, false))
        {
            RobotSave();

            return true;
        }
        return false;
    }

    /// <summary>
    /// Saves the joint information to the Inventor assembly file. Returns false if fails.
    /// </summary>
    public bool JointDataSave(Inventor.AssemblyDocument document)
    {
        Inventor.PropertySets propertySets = document.PropertySets;
        
        return JointDataSave(propertySets, SkeletonBase);
    }
    
    /// <summary>
    /// Recursive utility for JointDataSave.
    /// </summary>
    private bool JointDataSave(Inventor.PropertySets assemblyPropertySets, RigidNode_Base currentNode)
    {
        try
        {
            foreach (KeyValuePair<SkeletalJoint_Base, RigidNode_Base> connection in currentNode.Children)
            {
                SkeletalJoint_Base joint = connection.Key;
                RigidNode_Base child = connection.Value;

                // Name of the property set in inventor
                string setName = "bxd-jointdata-" + child.GetModelID();

                // Create the property set if it doesn't exist
                Inventor.PropertySet propertySet = Utilities.GetPropertySet(assemblyPropertySets, setName);

                // Add joint properties to set
                Utilities.SetProperty(propertySet, "has-driver", joint.cDriver != null);

                if (joint.cDriver != null)
                {
                    Utilities.SetProperty(propertySet, "driver-type", joint.cDriver.GetDriveType());
                    Utilities.SetProperty(propertySet, "driver-portA", joint.cDriver.portA);
                    Utilities.SetProperty(propertySet, "driver-portB", joint.cDriver.portB);
                    Utilities.SetProperty(propertySet, "driver-isCan", joint.cDriver.isCan);
                    Utilities.SetProperty(propertySet, "driver-lowerLimit", joint.cDriver.lowerLimit);
                    Utilities.SetProperty(propertySet, "driver-upperLimit", joint.cDriver.upperLimit);
                }

                // Recur along this child
                if (!JointDataSave(assemblyPropertySets, child))
                    return false; // If one of the children failed to save, then cancel the saving process
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Joint data could not be saved to the inventor file. The following error occured:\n" + e.Message);
            return false;
        }

        // Save was successful
        return true;
    }

    /// <summary>
    /// Loads the joint information from the Inventor assembly file. Returns false if fails.
    /// </summary>
    public bool JointDataLoad(Inventor.AssemblyDocument document)
    {
        Inventor.PropertySets propertySets = document.PropertySets;

        return JointDataLoad(propertySets, SkeletonBase);
    }

    /// <summary>
    /// Recursive utility for JointDataLoad.
    /// </summary>
    public bool JointDataLoad(Inventor.PropertySets assemblyPropertySets, RigidNode_Base currentNode)
    {
        foreach (KeyValuePair<SkeletalJoint_Base, RigidNode_Base> connection in currentNode.Children)
        {
            SkeletalJoint_Base joint = connection.Key;

            // Load joint information from a new property set, if a matching one exists

            if (!JointDataLoad(assemblyPropertySets, connection.Value))
                return false;
        }

        return false; // true
    }

    /// <summary>
    /// Get the desired folder to open from or save to
    /// </summary>
    /// <returns>The full path of the selected folder</returns>
    private string OpenFolderPath()
    {
        string dirPath = null;

        var dialogThread = new Thread(() =>
        {
            FolderBrowserDialog openDialog = new FolderBrowserDialog()
            {
                Description = "Select a Robot Folder"
            };
            DialogResult openResult = openDialog.ShowDialog();

            if (openResult == DialogResult.OK) dirPath = openDialog.SelectedPath;
        });

        dialogThread.SetApartmentState(ApartmentState.STA);
        dialogThread.Start();
        dialogThread.Join();

        return dirPath;
    }

    /// <summary>
    /// Warn the user that they are about to overwrite existing data
    /// </summary>
    /// <returns>Whether the user wishes to overwrite the data</returns>
    private bool WarnOverwrite()
    {
        DialogResult overwriteResult = MessageBox.Show("Really overwrite existing robot?", "Overwrite Warning", MessageBoxButtons.YesNo);

        if (overwriteResult == DialogResult.Yes) return true;
        else return false;
    }

    /// <summary>
    /// Warn the user that they are about to exit without unsaved work
    /// </summary>
    /// <returns>Whether the user wishes to continue without saving</returns>
    public bool WarnUnsaved()
    {
        DialogResult saveResult = MessageBox.Show("Do you want to save your work?", "Save", MessageBoxButtons.YesNoCancel);

        if (saveResult == DialogResult.Yes)
        {
            return RobotSave();
        }
        else if (saveResult == DialogResult.No)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Reload all panels with newly loaded robot data
    /// </summary>
    public void ReloadPanels()
    {
        jointEditorPane1.SetSkeleton(SkeletonBase);
        bxdaEditorPane1.loadModel(Meshes);
    }

    protected override void OnResize(EventArgs e)
    {
        SuspendLayout();

        base.OnResize(e);
        splitContainer1.Height = ClientSize.Height - 27;

        ResumeLayout();
    }

    /// <summary>
    /// Opens the <see cref="SetMassForm"/> form
    /// </summary>
    public void PromptRobotMass()
    {
        try
        {
            //TODO: Implement Value saving and loading
            SetMassForm massForm = new SetMassForm();

            massForm.ShowDialog();

            if (massForm.DialogResult == DialogResult.OK)
                TotalMass = massForm.TotalMass;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Opens the <see cref="PluginSettingsForm"/> form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void SettingsExporter_OnClick(object sender, System.EventArgs e)
    {
        try
        {
            //TODO: Implement Value saving and loading
            PluginSettingsForm eSettingsForm = new PluginSettingsForm();
    
            eSettingsForm.ShowDialog();
    
            //BXDSettings.Instance.AddSettingsObject("Plugin Settings", ExporterSettingsForm.values);
            PluginSettings = PluginSettingsForm.Values;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Runs the standalone Robot Viewer with and tells it to view the current robot
    /// </summary>
    /// <param name="settingsDir"></param>
    public void PreviewRobot(string settingsDir = null)
    {
        if(RMeta.ActiveDir != null)
        {
            Process.Start(Utilities.VIEWER_PATH, "-path \"" + RMeta.ActiveDir + "\"");
        }
        else
        {
            Process.Start(Utilities.VIEWER_PATH, "-path \"" + settingsDir + "\\" + RMeta.ActiveRobotName + "\"");
        }
    }

    /// <summary>
    /// Merges a node into the parent. Used during the one click export and the wizard.
    /// </summary>
    /// <param name="node"></param>
    public void MergeNodeIntoParent(RigidNode_Base node)
    {
        if (node.GetParent() == null)
            throw new ArgumentException("ERROR: Root node passed to MergeNodeIntoParent(RigidNode_Base)", "node");

        node.GetParent().ModelFullID += node.ModelFullID;

        //Get meshes for each node
        var childMesh = GetMesh(node);
        var parentMesh = GetMesh(node.GetParent());

        //Merge submeshes and colliders
        parentMesh.meshes.AddRange(childMesh.meshes);
        parentMesh.colliders.AddRange(childMesh.colliders);

        //Merge physics
        parentMesh.physics.Add(childMesh.physics.mass, childMesh.physics.centerOfMass);
        
        //Remove node from the children of its parent
        node.GetParent().Children.Remove(node.GetSkeletalJoint());
        Meshes.Remove(childMesh);
    }

    private BXDAMesh GetMesh(RigidNode_Base node)
    {
        return Meshes[SkeletonBase.ListAllNodes().IndexOf(node)];
    }

    #region OUTDATED EXPORTER METHODS
    /// <summary>
    /// Reset the <see cref="ExporterProgressWindow"/> progress bar
    /// </summary>
    public void ExporterReset()
    {
        exporter.ResetProgress();
    }

    public void ExporterOverallReset()
    {
        exporter.ResetOverall();
    }

    /// <summary>
    /// Set the length of the <see cref="ExporterProgressWindow"/> progress bar
    /// </summary>
    /// <param name="percentLength">The length of the bar in percentage points (0%-100%)</param>
    public void ExporterSetProgress(double percentLength)
    {
        exporter.AddProgress((int)Math.Floor(percentLength) - exporter.GetProgress());
    }

    /// <summary>
    /// Set the <see cref="ExporterProgressWindow"/> text after "Progress:"
    /// </summary>
    /// <param name="text">The text to add</param>
    public void ExporterSetSubText(string text)
    {
        exporter.SetProgressText(text);
    }

    public void ExporterSetMeshes(int num)
    {
        exporter.SetNumMeshes(num);
    }

    public void ExporterStepOverall()
    {
        exporter.AddOverallStep();
    }

    public void ExporterSetOverallText(string text)
    {
        exporter.SetOverallText(text);
    }

    private ExporterForm exporter = null;

    private void HelpTutorials_Click(object sender, EventArgs e)
    {
        Process.Start("http://bxd.autodesk.com/synthesis/tutorials-robot.html");
    }

    #endregion

}