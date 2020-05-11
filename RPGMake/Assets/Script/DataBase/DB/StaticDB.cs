using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//直接保持しているデータを確認される
//アクセスできるようにデータセットをpublicで持つ
public abstract class StaticDB: AbstractDB
{
    //データの変更が発生しないDB
    //画像データ等を保管する
}
