using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Manual_Explorer
{

    public enum WidgetType { BATTERY, PORT, INDICATOR };

    class RightSideBarManager
    {
        private Dictionary<string, TextBlock> textBlocks;
        private TextBlock batteryHolderText;
        private TextBlock totalBatteriesText;


        public RightSideBarManager(TextBlock AABatteriesText, TextBlock DBatteriesText, TextBlock batteryHolderText, TextBlock totalBatteriesText)
        {
            textBlocks = new Dictionary<string, TextBlock>();
            textBlocks["AA"] = AABatteriesText;
            textBlocks["D"] = DBatteriesText;
            this.batteryHolderText = batteryHolderText;
            this.totalBatteriesText = totalBatteriesText;
            ResetPanel();
        }

        public void ResetPanel()
        {
            batteryHolderText.Text = "0";
            totalBatteriesText.Text = "0";
            foreach(string widgetName in textBlocks.Keys)
            {
                textBlocks[widgetName].Text = "0";
            }
        }

        public void ChangeQuantity(string widgetToChange, int amount, WidgetType type)
        {
            switch (type)
            {
                case WidgetType.BATTERY:
                    UpdateBatteries(widgetToChange, amount);
                    break;
            }
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
