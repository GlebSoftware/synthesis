using System.Collections.Generic;
using InventorRobotExporter.Utilities;
using Inventor;
using Color = System.Drawing.Color;

namespace InventorRobotExporter.Managers
{
    public class HighlightManager
    {
        // Joint editor highlight
        private HighlightSet jointEditorHighlight;
        
        // Degrees of freedom highlight
        public bool DisplayJointHighlight
        {
            set
            {
                if (value)
                {
                    EnableDofHighlight(RobotExporterAddInServer.Instance.RobotDataManager);
                }
                else
                {
                    ClearDofHighlight();
                }
            }
        }

        private HighlightSet blueHighlightSet;
        private HighlightSet greenHighlightSet;
        private HighlightSet redHighlightSet;


        public void EnvironmentOpening(AssemblyDocument asmDocument)
        {
            blueHighlightSet = asmDocument.CreateHighlightSet();
            blueHighlightSet.Color = InventorUtils.GetInventorColor(Color.DodgerBlue);
            greenHighlightSet = asmDocument.CreateHighlightSet();
            greenHighlightSet.Color = InventorUtils.GetInventorColor(Color.LawnGreen);
            redHighlightSet = asmDocument.CreateHighlightSet();
            redHighlightSet.Color = InventorUtils.GetInventorColor(Color.Red);

            jointEditorHighlight = asmDocument.CreateHighlightSet();
            jointEditorHighlight.Color = InventorUtils.GetInventorColor(RobotExporterAddInServer.Instance.AddInSettingsManager.JointHighlightColor);
        }

        private void EnableDofHighlight(RobotDataManager robotDataManager)
        {
            if (robotDataManager.RobotBaseNode == null)
                return;

            var rootNodes = new List<RigidNode_Base> {robotDataManager.RobotBaseNode};
            var jointedNodes = new List<RigidNode_Base>();
            var problemNodes = new List<RigidNode_Base>();

            foreach (var node in robotDataManager.RobotBaseNode.ListAllNodes())
            {
                if (node == robotDataManager.RobotBaseNode) // Base node is already dealt with TODO: add ListChildren() to RigidNode_Base
                {
                    continue;
                }

                if (node.GetSkeletalJoint() == null || node.GetSkeletalJoint().cDriver == null) // TODO: Figure out how to identify nodes that aren't set up (highlight red)
                {
                    problemNodes.Add(node);
                }
                else
                {
                    jointedNodes.Add(node);
                }
            }

            jointEditorHighlight.Clear();
            InventorUtils.CreateHighlightSet(rootNodes, blueHighlightSet);
            InventorUtils.CreateHighlightSet(jointedNodes, greenHighlightSet);
            InventorUtils.CreateHighlightSet(problemNodes, redHighlightSet);
        }

        private void ClearDofHighlight()
        {
            blueHighlightSet.Clear();
            greenHighlightSet.Clear();
            redHighlightSet.Clear();
        }

        public void ClearJointHighlight()
        {
            jointEditorHighlight.Clear();
        }

        public void HighlightJoint(ComponentOccurrence componentOccurrence)
        {
            jointEditorHighlight.AddItem(componentOccurrence);
        }

        public void ClearAllHighlight()
        {
            DisplayJointHighlight = false;
            ClearJointHighlight();
        }

        public void SetJointHighlightColor(Color getInventorColor)
        {
            jointEditorHighlight.Color = InventorUtils.GetInventorColor(getInventorColor);
        }
    }
}