namespace Jagi.Mvc.Angular
{
    public enum FormGroupType
    {
        Default,
        Editor,
        Dropdown,
        Checkbox,
        Number,
        RadioButton,
        Textarea,
        Password,
        Hidden
    }

    public enum InputTag
    {
        Input,
        Select,
        Textarea
    }

    public class FormGroupLayout
    {
        public int LabelGrid;
        public int InputGrid;
        public int? FormGrid;

        /// <summary>
        /// 設定 AngularModel 預設的 Lable & Input grid length (兩者總和必須要小於 12)
        /// 也可以設定預設的 form-group grid length (選項)
        /// </summary>
        /// <param name="labelGridNumber">Label Grid length, 一般常用為 5</param>
        /// <param name="inputGridNumber">Input grid length, 一般常用為 7</param>
        /// <param name="formGrid">整個 Form-Group 的 Grid length，例如輸入 4 代表有三欄（12/4)，也可以不輸入</param>
        public FormGroupLayout(int labelGridNumber, int inputGridNumber, int? formGrid = null)
        {
            this.LabelGrid = labelGridNumber;
            this.InputGrid = inputGridNumber;
            this.FormGrid = formGrid;
        }
    }
}