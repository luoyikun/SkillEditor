#if UNITY_EDITOR
using UnityEngine;

namespace Amanda.EditorTable
{
    /// <summary>
    /// 编辑器配置表统一接口
    /// 所有自动生成的配置表类都需实现此接口
    /// </summary>
    public interface IEditorTable
    {
        /// <summary>
        /// 将配置表对象转换为制表符分隔的行文本
        /// </summary>
        /// <returns>制表符分隔的字符串</returns>
        string ToDataLine();
        //配置表的一行转为内容
        void LoadLine(string sLine);
    }
}
#endif