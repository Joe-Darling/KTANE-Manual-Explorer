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

        public void SelectedSerialNumberBox()
        {
            if (serialNumberText.Text.Equals("######"))
            {
                serialNumberText.Text = "";
            }
        }

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

        private void UpdateTextField(TextBlock textBlock, int amount)
        {
            int previousAmount = int.Parse(textBlock.Text);
            textBlock.Text = (previousAmount + amount).ToString();
        }

        private void UpdateBatteries(string batteryType, int amount)
        {
            if (textBlocks[batteryType].Text.Equals("0") && amount < 0)
            {
                return;
            }
            UpdateTextField(batteryHolderText, amount);

            if (batteryType.Equals("AA"))
            {
                amount *= 2;
            }
            UpdateTextField(textBlocks[batteryType], amount);
            UpdateTextField(totalBatteriesText, amount);
        }
    }
}
