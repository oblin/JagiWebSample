using System;

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
        /// <param name="labelGridNumber">Label Grid length, 一般常用為 6</param>
        /// <param name="inputGridNumber">Input grid length, 一般常用為 6</param>
        /// <param name="formGrid">整個 Form-Group 的 Grid length，例如輸入 4 代表有三欄（12/4)，也可以不輸入</param>
        public FormGroupLayout(int labelGridNumber, int inputGridNumber, int? formGrid = null)
        {
            SetValues(labelGridNumber, inputGridNumber, formGrid);
        }

        /// <summary>
        /// 使用標準的 grid 自行推算 label & input 寬度：
        /// Grid    Label   Input
        /// 3       8       4
        /// 4       6       6
        /// 6       4       8
        /// 8       3       9
        /// 12      2       10
        /// </summary>
        /// <param name="formGrid"></param>
        public FormGroupLayout(int formGrid)
        {
            if (formGrid != 3 && formGrid != 4 && formGrid != 6 && formGrid != 8 && formGrid != 12)
            {
                throw new ArgumentOutOfRangeException("僅接受 3, 4, 6, 8, 12 這五個參數");
            }

            switch (formGrid)
            {
                case 3:
                    SetValues(8, 4, 3);
                    break;
                case 4:
                    SetValues(6, 6, 4);
                    break;
                case 6:
                    SetValues(4, 8, 6);
                    break;
                case 8:
                    SetValues(3, 9, 8);
                    break;
                case 12:
                    SetValues(2, 10, 12);
                    break;
            }
        }

        private void SetValues(int labelGridNumber, int inputGridNumber, int? formGrid)
        {
            this.LabelGrid = labelGridNumber;
            this.InputGrid = inputGridNumber;
            this.FormGrid = formGrid;
        }
    }
}