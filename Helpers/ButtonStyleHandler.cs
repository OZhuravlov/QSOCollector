namespace QSOCollector.Helpers
{
    public static class ButtonStyleHandler
    {
        public static void Update(Button button, bool enabled, Color? backColor = null)
        {
            button.Enabled = enabled;
            Color color;
            FontStyle fontStyle;
            if (button.Enabled)
            {
                color = backColor == null ? Color.DarkCyan : backColor.Value;
                fontStyle = FontStyle.Bold;
            }
            else
            {
                color = backColor == null ? Color.Transparent : backColor.Value;
                fontStyle = FontStyle.Regular;
            }
            button.BackColor = color;
            button.Font = new Font(button.Font, fontStyle);
        }

    }
}
