using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NE4S.Exchange
{
    public interface IExchanger<T>
    {
        /// <summary>
        /// 外部IO（ファイルなど）からのデータを入力として、T型のデータ構造を構築します
        /// </summary>
        bool Import(out T t);

        /// <summary>
        /// T型のデータ構造を、外部IO（ファイルなど）に出力します
        /// </summary>
        bool Export(in T t);
    }

    public interface IExchanger<T, U>
    {
        /// <summary>
        /// T型のデータ構造を、U型のデータ構造に変換します
        /// </summary>
        bool Transpile(in T t, out U u);

        /// <summary>
        /// U型のデータ構造を、T型のデータ構造に変換します
        /// </summary>
        bool Transpile(in U u, out T t);
    }
}
