# RGGPack

UnityでRPGを作成するためのもの
2dを想定しているが3dでも使えるかもしれません

身内用の仮文書です


## イントロ

Scenes/TestSceneが初期セットアップ済みです

基本的なゲーム作成の流れ
データベース用のテキストファイルを更新
->DBOperatorのUpdateDataボタンを押してゲームに反映
->マップのPrefabにイベントを配置

## 各フォルダ説明　

各フォルダにあるものの説明。★★★だけ読めば作れるはず…
抜け漏れあり

Assets/Resource/DataBase以下

★★★

Assets/Resource/DataBaseフォルダの中にデータベース用のテキストファイルを保管する。

このテキストファイルに書き込むことでイベントやアイテムの追加、編集ができる

※ResourcesではなくResourceなので注意

データベース用のテキストファイルの書式は後述

★

DataBase_scriptableの中にテスト用のデータベースが配置してある

新しくデータベースを作成したいときはCreate->DataBases->DataBaseで作成可能

★★★

DataBase_scriptable/staticにリソース用のデータベースが配置してある

イベントで呼び出しするにはここに登録する必要がある。登録できるリソースは、画像、音楽、マップなど

基本的にsetNameとindexで呼び出しを行う

こちらも新しく作成したいときはCreate->DataBases->DataBaseで作成可能

★★★

Assets/Resource/DataBase/(テキストファイルの名前)フォルダにテキストをもとに作成されたデータが保管されている。イベントを設定する際はここのイベントを設定する。デフォルトだとフォルダの名前はEvent_testになっている。

=====================================

★

Asset/Resource/Image

Resource/Music

イラストや音楽を置いておくところ。別のところにおいてもエラーを吐いたりはしない

====================================

Assets/Resource/Prefabフォルダの中に必要なプレファブが配置してある

★★★
Assets/Resource/Prefab/Eventフォルダにイベント設定に必要なプレファブが配置してある。プレファブを配置し、
イベントデータの欄に呼び出したいイベントを入れることでイベントが呼び出せるようになる。

CheckEvent:前に立って決定ボタンを押すと呼び出されるイベント。呼び出し条件を満たしてない場合は調べることができない.

HitEvent:プレイヤーがぶつかった際に呼び出されるイベント。呼び出し条件を満たしてないと発生しない

★★
Assets/Resource/Prefabe/Improtantフォルダに重要なプレファブが配置してある。
新しくシーンを作成した場合はここにあるプレファブを配置する。デフォルトのシーンにはすでに置いてある

GameController,DBOperator,Playerをシーンに配置する

各プレファブの説明

DBOperator:イベント、アイテムなどのデータベースの更新を行うためのプレファブ

UpdateDataボタンを押すとテキストの情報をもとにデータベースが更新される。

GameController：ゲームの管理を行うプレファブ。最初にイベントを呼び出したい場合はGameControllerコンポーネントのFirst Eventにデータを登録する

★★★

Asset/Resource/Prefab/Mapフォルダにマップのプレファブが配置してある。新しくマップを作る際はここのものを参考にする。詳しい作り方は後述

★

Asset/Resource/Prefab/resourceフォルダにリソース関連のprefabが配置してある。GameControllerに含まれているので触れることはないはず

=============================================

★★

Asset/SaveData

セーブデータが保存されているフォルダ

データベース用テキストファイルのファイル名と同じ名前のものが作成されている

データの読み込みがうまくいっているか不安な場合は該当のセーブデータファイルを確認するといい。

デバッグの際にフラグの操作がしたい場合はここを書き換えるとできる。ただし、DataUpdateボタンを押すと初期化されるので注意

＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

★

Asset/Script

スクリプトが保存されている。基本的に変更することはないはず。もし変更したい場合は連絡して。


## マップの作成方法

１）	Assets/Prefab/Map内のMapPrefab_templateをシーンにドラッグ

２）	Hierachyビューのプレファブを右クリックしUnpack prefab completelyを選択しプレファブでなくする。文字の色が青じゃなくなれば成功

３）	名前を付けてプロジェクトビューにドラッグ（プレファブの作成）。以降の変更はプレファブ内で行う。
プレファブ内で変更を行うには、対象のプレファブをダブルクリックする。

４）	作成したマップをデータベースに登録する。

Assets/Resource/DataBaseDataBase_scriptable/staticの中にあるマップのデータベースに登録する。

５）	Collidersの下に当たり判定をつける（BoxCollider2Dなど）

Imageの下に画像を張る。

moveTargetをマップに移動した際の初期位置に設定する


## データベース用テキストファイルの書式

現状、アイテム、フラグ、スキル、キャラクター、プレイヤー、エネミーセットのデータが存在。

各ファイルにはサンプルデータがあるのでそれを参考にして

書式

Type(タイプ名)　//そのファイルが何のデータを表しているか。

Id: アイディー名 //そのデータを特定するための文字列。イベント呼びしの時に使う

	メンバー名 データ内容//そのデータの変数名とそれに対応するデータ。必要数が必ず必要


・Idはかぶっちゃダメなので気を付けて。

サンプル

Type(Item)

Id: test1

	displayName テスト1
  
	explanation データの登録テストです
  
	maxNum 20
  
	haveNum 0
  

## イベントデータの書式

Type(Event)

id: イベント名//そのデータを特定するための文字列。イベント呼びしの時に使う

	text{//イベントを実行する際に読み込まれるテキスト。書式は後述

	}
  
	term{//呼び出し条件満たしている場合にイベントが呼び出される
  
		ormode true
    
		検索するid 対象の変数名 データの値　比較演算子
    
		比較演算子は more less equal
    
	}
  
	next{//次に呼び出すイベント。複数設定する場合は条件を満たしている最も↑のものが呼ばれる
  
		イベント名
    
	}

・{}の中が存在しない場合はその項目を書かない。

Type(Event)

id: testevent

	text{
  
		$
    
		文章を入力
    
		$
    
	}
  
	term{
  
		ormode true
    
		id memeber 1 less
    
		flagId flagNum 0 more
    
		itemId haveNum 1 equal
    
	}
  
	next{
  
		moveToClass
    
		door_class
    
	}

## イベントデータのテキストの書式

$

ドルマークで一ページ区切り

特殊文字ナシならテキストとして表示

$

ここから特殊文字の説明


ドルの後に#で特殊文字

$#name[nameData]

nameはテキストの前に名前の表示を追加できる

nameDataに表示したい名前を入れる

例　

$#name[京子]

私京子！ 

↑これでテキストの前に名前が表示される

$#branch

branchData1

branchData2

$#1

１を選んだよ

$#2

2を選んだよ

$#2

２のイベントを継続するよ

branchは選択肢を表示できる

branchDataに選択肢の内容を入れる

選択した後は$#選んだ番号　でそこが再生される

$#flag[flagName] num

flagはフラグの値を変更できる

flagNameに変更したいflagのid

numに変更後の数値を入れる

$#item[itemName] num

itemはアイテムの取得減少などができる

itemNameに変更したいitemのid

nuumに変更後の値を入れる

$#skill[playerName,skillName]

Skillはスキルの習得ができる

playerNameは習得するキャラクターのid(現状kouのみ)

skillNameは習得するスキルのid

すでに習得しているスキルは習得不可

スキルを習得したときのメッセージが自動で出たりはしないので手動でメッセージ書いて（そのうち実装予定）

↓具体例

$#skill[kou,fire]

$

コウはファイヤを習得した！（←このメッセージが自動で出ない）

$

$#battle[setName]

Battleは戦闘を開始することができる

setNameに呼び出したいenemySetのid

$#map[mapName]

mapはマップの遷移ができる

mapNameに移動したいmapのidを入れる

$image[setName,num] pos (r)

imageは画像を変更することができる

setNameは画像が登録されているセットのid

numはセット内での番号

pos はどこに表示するか　（back 背景 center 真ん中）

例　$#image[kyoko,0] center ならkyokoの0番の画像を真ん中に表示

また,image[reset] pos　で指定した位置の画像を消すことができる

最後のrはあってもなくてもいい　ある場合は反転して表示

$#music[setName,num]

musicは音楽を変更することができる

setNameは音楽が登録されているセットのid

numはセット内での番号

$#load[command]

loadは暗転させることができる

commandは暗転or暗転終了 (black 暗転 clear 暗転終了)

例 $#load[black] 暗転する　

$#wait[num]

waitは処理を一定時間止めることができる

num/100秒停止する　例 wait[100] なら1秒待つ





## データベースデータの説明

説明が必要そうなもののみ記載

・skill

	skillName スキル名
  
	target 効果対象　詳細は下記
  
	useResource 使用するリソース（spなど）　詳細は下記
  
	useNum 使用するリソース量
  
	targetResource　何に効果を与えるか　hpならhpにダメージor回復　spなら　同様
  
	rate　効果倍率　(rate÷100)×攻撃力が効果量になる　回復なら回復量=効果量
  
targetの数字の意味

	NONE =-1,
  
	SELF=0,
  
	FRIEND_SOLO=1,
  
	FRIEND_ALL=2,
  
	FRIEND_RANDOM=3,
  
	ENEMY_SOLO=4,
  
	ENEMY_ALL=5,
  
	ENEMY_RANDOM=6
useResource, targetResource　の数字の意味

	NONE = 0,
  
	SP = 1,
  
	HP = 2,
  
## Text

テキスト内に[]を書くことで指定した文章を入れ込むことができる

[item,getItem] 入手アイテムの名前を取得

テキスト内の<>は関数呼び出し処理なので使わないで
