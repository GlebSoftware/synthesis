﻿using System;
using System.Windows.Forms;
using InventorRobotExporter.GUI.Editors.JointSubEditors;
using InventorRobotExporter.Utilities;

namespace InventorRobotExporter.GUI.Editors.JointEditor
{
    public partial class AdvancedJointSettingsForm : Form
    {
        private readonly SkeletalJoint_Base joint;
        public double GearRatio = 1;
        public int PortId = 3;
        public bool IsCan = true;

        public AdvancedJointSettingsForm(SkeletalJoint_Base passJoint)
        {
            joint = passJoint;
            InitializeComponent();
            UpdateSensorList();
            RestoreFields();
        }

        public void DoLayout(bool isDriveTrainWheel)
        {
            if (isDriveTrainWheel)
            {
                portBox.Visible = false;
            }
            else
            {
                portBox.Visible = true;
            }
        }

        private void RestoreFields()
        {
            gearRatioInput.Value = joint.cDriver == null ? (decimal) GearRatio : (decimal) (joint.cDriver.OutputGear / joint.cDriver.InputGear);
            portInput.Value = Math.Max(3, joint.cDriver?.port1 ?? PortId);
            portTypeInput.SelectedItem = joint.cDriver != null && !joint.cDriver.isCan ? "PWM" : "CAN";
            sensorListView.ColumnWidthChanging += sensorListView_ColumnWidthChanging;
        }

        private void UpdateSensorList()
        {
            sensorListView.Items.Clear();

            foreach (RobotSensor sensor in joint.attachedSensors)
            {
                if (sensor.type.Equals(RobotSensorType.ENCODER))
                {// if the sensor is an encoder show both ports
                    ListViewItem item = new ListViewItem(new[] {
                    char.ToUpper(sensor.type.ToString()[0]) + sensor.type.ToString().Substring(1).ToLower(),
                        sensor.portA.ToString(), sensor.portB.ToString()});
                    item.Tag = sensor;
                    sensorListView.Items.Add(item);
                } else
                {
                    ListViewItem item = new ListViewItem(new[] {
                    char.ToUpper(sensor.type.ToString()[0]) + sensor.type.ToString().Substring(1).ToLower(),
                        sensor.portA.ToString()});
                    item.Tag = sensor;
                    sensorListView.Items.Add(item);

                }
            }
        }

        private void sensorListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = sensorListView.Columns[e.ColumnIndex].Width;
        }

        private void AddSensorButton_Click(object sender, EventArgs e)
        {
            JointSensorEditorForm sensorEditorForm = new JointSensorEditorForm(joint, joint.attachedSensors.IndexOf(
                sensorListView.SelectedItems.Count > 0 &&
                sensorListView.SelectedItems[0].Tag is RobotSensor ?
                    (RobotSensor)sensorListView.SelectedItems[0].Tag : null));
            sensorEditorForm.ShowDialog(this);
            UpdateSensorList();
        }

        private void RemoveSensorButton_Click(object sender, EventArgs e)
        {
            if (sensorListView.SelectedItems.Count > 0 && sensorListView.SelectedItems[0].Tag is RobotSensor)
            {
                joint.attachedSensors.Remove((RobotSensor)sensorListView.SelectedItems[0].Tag);
                UpdateSensorList();
            }
        }

        private void SensorListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            addSensorButton.Text = joint.attachedSensors.IndexOf(
                                       sensorListView.SelectedItems.Count > 0 &&
                                       sensorListView.SelectedItems[0].Tag is RobotSensor ?
                                           (RobotSensor)sensorListView.SelectedItems[0].Tag : null) >= 0 ? "Edit Sensor" : "Add Sensor";
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            GearRatio = (double) gearRatioInput.Value;
            PortId = (int) portInput.Value;
            IsCan = (string) portTypeInput.SelectedItem == "CAN";
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RestoreFields();
            Close();
        }
    }
}
