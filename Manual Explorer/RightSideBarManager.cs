using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Manual_Explorer
{

    public enum WidgetType { BATTERY, PORT };

    class RightSideBarManager
    {
        private Dictionary<string, TextBlock> textBlocks;
        private TextBox serialNumberText;
        private TextBlock batteryHolderText;
        private TextBlock totalBatteriesText;
        private TextBlock totalPortsText;
        private TextBlock litIndicatorText;
        private TextBlock unlitIndicatorText;
        private Grid rightPanel;


        public RightSideBarManager(TextBox serialNumberText, TextBlock AABatteriesText, TextBlock DBatteriesText, TextBlock batteryHolderText, TextBlock totalBatteriesText,
            TextBlock DVIText, TextBlock parallelText, TextBlock ps2Text, TextBlock rj45Text, TextBlock serialText, TextBlock rcaText, TextBlock totalPortsText, TextBlock litIndicatorText,
            TextBlock unlitIndicatorText, Grid rightPanel)
        {
            // We create a dictionary containing the widget names and the corresponding text block that cotains the count for that widget.
            textBlocks = new Dictionary<string, TextBlock>();
            textBlocks["AA"] = AABatteriesText;
            textBlocks["D"] = DBatteriesText;
            textBlocks["DVI"] = DVIText;
            textBlocks["Parallel"] = parallelText;
            textBlocks["PS2"] = ps2Text;
            textBlocks["RJ45"] = rj45Text;
            textBlocks["Serial"] = serialText;
            textBlocks["RCA"] = rcaText;

            this.serialNumberText = serialNumberText;
            this.batteryHolderText = batteryHolderText;
            this.totalBatteriesText = totalBatteriesText;
            this.totalPortsText = totalPortsText;
            this.litIndicatorText = litIndicatorText;
            this.unlitIndicatorText = unlitIndicatorText;
            this.rightPanel = rightPanel;
            ResetPanel();
        }

        /// <summary>
        /// Resets all control elements on the right page to their default state.
        /// </summary>
        public void ResetPanel()
        {
            var checkBoxes = rightPanel.Children.OfType<CheckBox>();
            foreach (CheckBox checkBox in checkBoxes)
            {
                checkBox.IsChecked = false;
            }

            foreach (string widgetName in textBlocks.Keys)
            {
                textBlocks[widgetName].Text = "0";
            }

            serialNumberText.Text = "######";
            batteryHolderText.Text = "0";
            totalBatteriesText.Text = "0";
            totalPortsText.Text = "0";
            litIndicatorText.Text = "0";
            unlitIndicatorText.Text = "0";
        }

        /// <summary>
        /// Change the control elements text to reflect the change entered by the user.
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="amount"></param>
        private void UpdateTextField(TextBlock textBlock, int amount)
        {
            int previousAmount = int.Parse(textBlock.Text);
            textBlock.Text = (previousAmount + amount).ToString();
        }

        /// <summary>
        /// Updates the lit/unlit fields based on the indicator checkbox toggled.
        /// </summary>
        /// <param name="isLit"></param>
        /// <param name="isChecked"></param>
        public void ToggledIndicator(bool isLit, bool isChecked)
        {
            int val = isChecked ? 1 : -1;
            if (isLit)
            {
                UpdateTextField(litIndicatorText, val);
            }
            else
            {
                UpdateTextField(unlitIndicatorText, val);
            }
        }

        /// <summary>
        /// Clears serial information if in its default state.
        /// </summary>
        public void SelectedSerialNumberBox()
        {
            if (serialNumberText.Text.Equals("######"))
            {
                serialNumberText.Text = "";
            }
        }

        /// <summary>
        /// Determines the widget that needs to be updated and calls the appropriate function to update that widget.
        /// </summary>
        /// <param name="widgetToChange"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        public void ChangeQuantity(string widgetToChange, int amount, WidgetType type)
        {
            switch (type)
            {
                case WidgetType.BATTERY:
                    UpdateBatteries(widgetToChange, amount);
                    break;
                case WidgetType.PORT:
                    UpdatePorts(widgetToChange, amount);
                    break;
                default:
                    throw new ArgumentException("The Widget type: " + type + " is not being handled");
            }
        }

        private void UpdatePorts(string portType, int amount)
        {
            if (textBlocks[portType].Text.Equals("0") && amount < 0)
            {
                return;
            }
            UpdateTextField(textBlocks[portType], amount);
            UpdateTextField(totalPortsText, amount);
        }

        private void UpdateBatteries(string batteryType, int amount)
        {
            if (textBlocks[batteryType].Text.Equals("0") && amount < 0)
            {
                return;
            }
            // First increment battery holders by the base amount
            UpdateTextField(batteryHolderText, amount);

            // Then if the battery type is AA, double the amount
            if (batteryType.Equals("AA"))
            {
                amount *= 2;
            }

            // Finally update the corresponding text area and total batteries with the resulting amount
            UpdateTextField(textBlocks[batteryType], amount);
            UpdateTextField(totalBatteriesText, amount);
        }
    }
}
