using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMxStructure.PMX;

namespace PMXLoadingChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = System.Console.Out;
            if (args.Length < 1)
            {
                console.WriteLine("無効な引数です。");
                return;
            }
            // 読み込むPMXモデルのフルパスを設定する
            string path = args[0];
            // string path = @"C:\hoge\mikumiku\mikusan.pmx"    //←直接記述（ハードコーディング）の例

            if(System.IO.File.Exists(path) == false)
            {
                console.WriteLine("存在しないファイルが指定されました。");
                return;
            }
            console.WriteLine("読み込みテストを開始します。");
            //PMX pmx = new PMX();
            string[] targetVersions = new string[] { "2.0", "2.1" };
            Header Header;
            ModelInfo ModelInfo = new ModelInfo();
            List<VertexInfo> Vertex = new List<VertexInfo>();
            List<VertexIndexInfo> VertexIndex = new List<VertexIndexInfo>();
            List<TextureInfo> Texture = new List<TextureInfo>();
            List<MaterialInfo> Materail = new List<MaterialInfo>();
            List<BoneInfo> Bone = new List<BoneInfo>();
            List<MorphInfo> Morph = new List<MorphInfo>();
            List<SystemViewInfo> SystemView = new List<SystemViewInfo>();
            List<RigidBodyInfo> Rigidbody = new List<RigidBodyInfo>();
            List<ConstraintInfo> Constraint = new List<ConstraintInfo>();
            List<SoftBodyInfo> Softbody = new List<SoftBodyInfo>();

            using (BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                Header = new Header();
                try
                {
                    Header.FromBinary(null, br);
                    console.WriteLine("ヘッダ読み込みに成功");

                    ModelInfo.FromBinary(Header, br);
                    console.WriteLine("モデル情報読み込みに成功");

                    Vertex = PMXBinaryReadWriteUtil.FromBinary<VertexInfo>(Header, br, new Func<IBinaryTransable>(() => { return new VertexInfo(); }));
                    console.WriteLine("頂点情報読み込みに成功");

                    VertexIndex = PMXBinaryReadWriteUtil.FromBinary<VertexIndexInfo>(Header, br, new Func<IBinaryTransable>(() => { return new VertexIndexInfo(); }));
                    console.WriteLine("頂点インデックス読み込みに成功");

                    Texture = PMXBinaryReadWriteUtil.FromBinary<TextureInfo>(Header, br, new Func<IBinaryTransable>(() => { return new TextureInfo(); }));
                    console.WriteLine("テクスチャ読み込みに成功");

                    Materail = PMXBinaryReadWriteUtil.FromBinary<MaterialInfo>(Header, br, new Func<IBinaryTransable>(() => { return new MaterialInfo(); }));
                    console.WriteLine("マテリアル情報読み込みに成功");

                    Bone = PMXBinaryReadWriteUtil.FromBinary<BoneInfo>(Header, br, new Func<IBinaryTransable>(() => { return new BoneInfo(); }));
                    console.WriteLine("ボーン読み込みに成功");

                    Morph = PMXBinaryReadWriteUtil.FromBinary<MorphInfo>(Header, br, new Func<IBinaryTransable>(() => { return new MorphInfo(); }));
                    console.WriteLine("モーフ読み込みに成功");

                    SystemView = PMXBinaryReadWriteUtil.FromBinary<SystemViewInfo>(Header, br, new Func<IBinaryTransable>(() => { return new SystemViewInfo(); }));
                    console.WriteLine("システム名称読み込みに成功");

                    Rigidbody = PMXBinaryReadWriteUtil.FromBinary<RigidBodyInfo>(Header, br, new Func<IBinaryTransable>(() => { return new RigidBodyInfo(); }));
                    console.WriteLine("剛体読み込みに成功");

                    Constraint = PMXBinaryReadWriteUtil.FromBinary<ConstraintInfo>(Header, br, new Func<IBinaryTransable>(() => { return new ConstraintInfo(); }));
                    console.WriteLine("ジョイント読み込みに成功");

                    if (Header.Version.ToString().Equals(targetVersions[1]))
                    {
                        Softbody = PMXBinaryReadWriteUtil.FromBinary<SoftBodyInfo>(Header, br, new Func<IBinaryTransable>(() => { return new SoftBodyInfo(); }));
                        console.WriteLine("ソフトボディ読み込みに成功");
                    }
                }
                catch (Exception ex)
                {
                    console.WriteLine(ex.Message);
                    console.WriteLine("読み込みを中止します。");
                    return;
                }
                console.WriteLine(string.Format("モデル名（JP）：{0}", ModelInfo.Name.Japanese));
                console.WriteLine(string.Format("モデル名（EN）：{0}", ModelInfo.Name.English));
                console.WriteLine(string.Format("頂点数：{0}", Vertex.Count));
                console.WriteLine(string.Format("頂点インデックス数：{0}", VertexIndex.Count));
                console.WriteLine(string.Format("テクスチャ数：{0}", Texture.Count));
                console.WriteLine(string.Format("マテリアル数：{0}", Materail.Count));
                console.WriteLine(string.Format("ボーン数：{0}", Bone.Count));
                console.WriteLine(string.Format("モーフ数：{0}", Morph.Count));
                console.WriteLine(string.Format("システム名称数：{0}", SystemView.Count));
                console.WriteLine(string.Format("剛体数：{0}", Rigidbody.Count));
                console.WriteLine(string.Format("ジョイント数：{0}", Constraint.Count));
                console.WriteLine(string.Format("ソフトボディ数：{0}", Softbody.Count));

                try
                {
                    System.IO.FileInfo fi = new FileInfo(path);
                    string directoryPath = fi.Directory.FullName;

                    // ヘッダ情報出力
                    using (StreamWriter sw = new StreamWriter(new FileStream(string.Format(@"{0}\{1}", directoryPath, "header.txt"), FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
                    {
                        sw.WriteLine(string.Format("{0},{1},{2}", Header.Magic[0], Header.Magic[1], Header.Magic[2]));
                        StringBuilder sb = new StringBuilder();
                        foreach (var b in Header.DataField)
                        {
                            sb.Append(b.ToString());
                        }
                        sw.WriteLine(sb.ToString());
                        sw.WriteLine(Header.Version);
                    }

                    // モデル名、コメント出力
                    using (StreamWriter sw = new StreamWriter(new FileStream(string.Format(@"{0}\{1}", directoryPath, "info.txt"), FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
                    {
                        // sw.WriteLine()はファイルへ1行文字列（テキスト）を書き込んで改行する
                        sw.WriteLine(ModelInfo.Name.Japanese);
                        sw.WriteLine(ModelInfo.Name.English);
                        sw.WriteLine(ModelInfo.Comment.Japanese);
                        sw.WriteLine(ModelInfo.Comment.English);
                    }

                    // 頂点情報出力　作成途中
                    using (StreamWriter sw = new StreamWriter(new FileStream(string.Format(@"{0}\{1}", directoryPath, "vertex.txt"), FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
                    {
                        // リスト要素全てをループ
                        foreach (var aVertex in Vertex)
                        {
                            // StringBuilderは文字列を連結させる
                            // sb.Append(文字列）とすると内部的にどんどん文字列を連結して蓄えていく←改行はされない！
                            // sb.ToString()とすると最終的に長い一つの文字列として取り出せる
                            StringBuilder sb = new StringBuilder();

                            // string.Formatは文字列を整形する
                            // {0},{1},{2}はカンマ区切りで{0}{1}{2}の位置にデータを格納して文字列にする
                            // 例：string.Format("{0},{1},{2}","一番目","二番目","三番目");は
                            // "一番目,二番目,三番目"と出力される
                            // ↓1行目は頂点座標XYZをカンマ区切りにしてStringBuilderに追加している。文字列最後の,（{2}の後ろ）2行目の法線情報連結用
                            sb.Append(string.Format("{0},{1},{2},", aVertex.Position.X, aVertex.Position.Y, aVertex.Position.Z));
                            sb.Append(string.Format("{0},{1},{2},", aVertex.Normal.X, aVertex.Normal.Y, aVertex.Normal.Z));
                            sb.Append(string.Format("{0},{1},", aVertex.UV.U, aVertex.UV.V));
                            switch (aVertex.WeightType)
                            {
                                case VertexInfo.WeightTypes.BDEF1:
                                    //sb.Append(string.Format("{0},{1},", aVertex.WeightBDEF., aVertex.UV.V));
                                    break;
                            }

                            // カンマ区切りで整形済み頂点データをファイルへ1行分書き込む
                            sw.WriteLine(sb.ToString());
                        }
                    }
                    // 頂点インデックス出力
                    // 略

                    // テクスチャ情報出力
                    //                                                                               出力フォルダのパス、出力ファイル名
                    using (StreamWriter sw = new StreamWriter(new FileStream(string.Format(@"{0}\{1}", directoryPath, "texture.txt"), FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
                    {
                        // リスト要素全てをループ
                        foreach (var aTextureInfo in Texture)
                        {
                            // 1つのテクスチャパスをファイルへ1行分書き込む
                            sw.WriteLine(aTextureInfo.TexturePath);
                        }
                    }
                    // マテリアル情報出力
                    using (StreamWriter sw = new StreamWriter(new FileStream(string.Format(@"{0}\{1}", directoryPath, "material.txt"), FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
                    {
                        
                        // 略
                    }
                    // ボーン情報出力
                    // モーフ情報出力
                    // システム名称出力　枠名とか
                    // 剛体情報出力
                    // ジョイント情報出力
                    // ソフトボディ出力 ソフトボディは情報が無い場合もあります
                }
                catch
                {
                    console.WriteLine("ファイル出力に失敗しました。");
                }
            }
            console.WriteLine("読み込みテスト完了しました。");
        }
    }
}