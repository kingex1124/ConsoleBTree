using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBTree
{
    class Program
    {
        // https://iter01.com/420924.html
        static void Main(string[] args)
        {
            //Program pg = new Program();
            var bTree = new BTree();

            bTree.Insert(4);
            bTree.Insert(5);
            bTree.Insert(6);
            bTree.Insert(1);
            bTree.Insert(2);
            bTree.Insert(3);
            bTree.Insert(10);
            bTree.Insert(11);
            bTree.Insert(12);
            bTree.Insert(7);
            bTree.Insert(8);
            bTree.Insert(9);
            bTree.Insert(13);
            bTree.Insert(14);
            bTree.Insert(18);
            bTree.Insert(19);
            bTree.Insert(20);
            bTree.Insert(15);
            bTree.Insert(16);
            bTree.Insert(17);

            Console.WriteLine("輸出排序後鍵值");
            bTree.PrintByIndex();
        }
    }

    public class Consts
    {
        public const int M = 3;                  // B樹的最小度數
        public const int KeyMax = 2 * M - 1;     // 節點包含關鍵字的最大個數
        public const int KeyMin = M - 1;         // 非根節點包含關鍵字的最小個數
        public const int ChildMax = KeyMax + 1;  // 孩子節點的最大個數
        public const int ChildMin = KeyMin + 1;  // 孩子節點的最小個數
    }

    public class BTreeNode
    {
        private bool leaf;
        public int[] keys;
        public int keyNumber;
        public BTreeNode[] children;
        public int blockIndex;
        public int dataIndex;

        public BTreeNode(bool leaf)
        {
            this.leaf = leaf;
            keys = new int[Consts.KeyMax];
            children = new BTreeNode[Consts.ChildMax];
        }

        /// <summary>在未滿的節點中插入鍵值</summary>
        /// <param name="key">鍵值</param>
        public void InsertNonFull(int key)
        {
            var index = keyNumber - 1;

            if (leaf == true)
            {
                // 找到合適位置,並且移動節點鍵值騰出位置
                while (index >= 0 && keys[index] > key)
                {
                    keys[index + 1] = keys[index];
                    index--;
                }

                // 在index後邊新增鍵值
                keys[index + 1] = key;
                keyNumber = keyNumber + 1;
            }
            else
            {
                // 找到合適的子孩子索引
                while (index >= 0 && keys[index] > key) index--;

                // 如果孩子節點已滿
                if (children[index + 1].keyNumber == Consts.KeyMax)
                {
                    // 分裂該孩子節點
                    SplitChild(index + 1, children[index + 1]);

                    // 分裂後中間節點上跳父節點
                    // 孩子節點已經分裂成2個節點,找到合適的一個
                    if (keys[index + 1] < key) index++;
                }

                // 插入鍵值
                children[index + 1].InsertNonFull(key);
            }
        }

        /// <summary>分裂節點</summary>
        /// <param name="childIndex">孩子節點索引</param>
        /// <param name="waitSplitNode">待分裂節點</param>
        public void SplitChild(int childIndex, BTreeNode waitSplitNode)
        {
            var newNode = new BTreeNode(waitSplitNode.leaf);
            newNode.keyNumber = Consts.KeyMin;

            // 把待分裂的節點中的一般節點搬到新節點
            for (var j = 0; j < Consts.KeyMin; j++)
            {
                newNode.keys[j] = waitSplitNode.keys[j + Consts.ChildMin];

                // 清0
                waitSplitNode.keys[j + Consts.ChildMin] = 0;
            }

            // 如果待分裂節點不是也只節點
            if (waitSplitNode.leaf == false)
            {
                for (var j = 0; j < Consts.ChildMin; j++)
                {
                    // 把孩子節點也搬過去
                    newNode.children[j] = waitSplitNode.children[j + Consts.ChildMin];

                    // 清0
                    waitSplitNode.children[j + Consts.ChildMin] = null;
                }
            }

            waitSplitNode.keyNumber = Consts.KeyMin;

            // 拷貝一般鍵值到新節點
            for (var j = keyNumber; j >= childIndex + 1; j--)
                children[j + 1] = children[j];

            children[childIndex + 1] = newNode;
            for (var j = keyNumber - 1; j >= childIndex; j--)
                keys[j + 1] = keys[j];

            // 把中間鍵值上跳至父節點
            keys[childIndex] = waitSplitNode.keys[Consts.KeyMin];

            // 清0
            waitSplitNode.keys[Consts.KeyMin] = 0;

            // 根節點鍵值數自加
            keyNumber = keyNumber + 1;
        }

        /// <summary>根據節點索引順序列印節點鍵值</summary>
        public void PrintByIndex()
        {
            int index;
            for (index = 0; index < keyNumber; index++)
            {
                // 如果不是葉子節點, 先列印葉子子節點. 
                if (leaf == false) children[index].PrintByIndex();

                Console.Write("{0} ", keys[index]);
            }

            // 列印孩子節點
            if (leaf == false) children[index].PrintByIndex();
        }

        /// <summary>查詢某鍵值是否已經存在樹中</summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public BTreeNode Find(int key)
        {
            int index = 0;
            while (index < keyNumber && key > keys[index]) index++;

            // 該key已經存在, 返回該索引位置節點
            if (keys[index] == key) return this;

            // key 不存在,並且節點是葉子節點
            if (leaf == true) return null;

            // 遞迴在孩子節點中查詢
            return children[index].Find(key);
        }
    }

    public class BTree
    {
        public BTreeNode Root { get; private set; }

        public BTree() { }

        /// <summary>根據節點索引順序列印節點鍵值</summary>
        public void PrintByIndex()
        {
            if (Root == null)
            {
                Console.WriteLine("空樹");
                return;
            }

            Root.PrintByIndex();
        }

        /// <summary>查詢某鍵值是否已經存在樹中</summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public BTreeNode Find(int key)
        {
            if (Root == null) return null;

            return Root.Find(key);
        }

        /// <summary>新增B樹節點鍵值</summary>
        /// <param name="key">鍵值</param>
        public void Insert(int key)
        {
            if (Root == null)
            {
                Root = new BTreeNode(true);
                Root.keys[0] = key;
                Root.keyNumber = 1;
                return;
            }

            if (Root.keyNumber == Consts.KeyMax)
            {
                var newNode = new BTreeNode(false);

                newNode.children[0] = Root;
                newNode.SplitChild(0, Root);

                var index = 0;
                if (newNode.keys[0] < key) index++;

                newNode.children[index].InsertNonFull(key);
                Root = newNode;
            }
            else
            {
                Root.InsertNonFull(key);
            }
        }
    }
}
