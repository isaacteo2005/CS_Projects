
using HW2;
using System.Windows.Forms;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        /// summary: normal case for hash set,param: none, return: none, exception: none
        public void TestHashSetNormal()
        {
            //normal case
            var form = new Form1();
            int[] arr1 = [5, 3, 5, -1, 3, 0];
            int num1 = form.UniqueWithHashSet(arr1);
            Assert.AreEqual(num1, 4);

        }
        /// summary: edge case for hash set with all duplicate elements,param: none, return: none, exception: none
        [Test]
        public void TestHashSetAllDupes()
        {
            // all duplicates
            var form = new Form1();
            int[] arr2 = [5, 5, 5, 5, 5];
            int num2 = form.UniqueWithHashSet(arr2);
            Assert.AreEqual(num2, 1);

        }
        /// <summary>
        /// edge case with no elements,param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestHashSetNoElems()
        {
            //no elements (edge case)
            var form = new Form1();
            int[] arr4 = [];
            int num4 = form.UniqueWithHashSet(arr4);
            Assert.AreEqual(num4, 0);
        }
        /// <summary>
        /// expcetion case that tests hashset and low storage method with large array, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestHashSetLowStorageLarge()
        {
            var form = new Form1();
            Random ran = new Random();
            int[] arr = new int[100000];
            for (int i = 0; i < 100000; i++)
            {
                arr[i] = ran.Next(0, 20001);
            }
            Assert.AreEqual(form.UniqueWithHashSet(arr), form.UniqueWithLowStorage(arr));
        }
        /// <summary>
        /// expcetion case that tests hashset and sorted method with large array, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestHashSetSortedLarge()
        {
            var form = new Form1();
            Random ran = new Random();
            int[] arr = new int[100000];
            for (int i = 0; i < 100000; i++)
            {
                arr[i] = ran.Next(0, 20001);
            }
            Assert.AreEqual(form.UniqueWithHashSet(arr), form.UniqueWithSorted(arr));
        }
        [Test]
        /// summary: normal cases for test low storage,param: none, return: none, exception: none
        public void TestLowStorageNormal()
        {
            //normal case
            var form = new Form1();
            int[] arr1 = [5, 3, 5, -1, 3, 0];
            int num1 = form.UniqueWithLowStorage(arr1);
            Assert.AreEqual(num1, 4);

        }
        /// <summary>
        /// edge case for test low storage with all dupes, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestLowStorageAllDupes()
        {
            var form = new Form1();
            // all duplicates
            int[] arr2 = [5, 5, 5, 5, 5];
            int num2 = form.UniqueWithLowStorage(arr2);
            Assert.AreEqual(num2, 1);

        }
        /// <summary>
        /// edge case for low storage method, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestLowStorageNoElems()
        {
            var form = new Form1();
            //no elements (edge case)
            int[] arr4 = [];
            int num4 = form.UniqueWithLowStorage(arr4);
            Assert.AreEqual(num4, 0);

        }
        [Test]
        /// summary: test cases for test sorted,param: none, return: none, exception: none
        public void TestSortedNormal()
        {
            //normal case
            var form = new Form1();
            int[] arr1 = [5, 3, 5, -1, 3, 0];
            int num1 = form.UniqueWithSorted(arr1);
            Assert.AreEqual(num1, 4);

        }
        /// <summary>
        /// Edge case testing all duplicates, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestSortedAllDupes()
        {
            var form = new Form1();
            // all duplicates
            int[] arr2 = [5, 5, 5, 5, 5];
            int num2 = form.UniqueWithSorted(arr2);
            Assert.AreEqual(num2, 1);
        }
        /// <summary>
        /// Edge cases for testing no dupes and already sorted, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestSortedNoDupes()
        {
            var form = new Form1();
            //no duplicates + already sorted
            int[] arr3 = [1, 2, 3, 4, 5];
            int num3 = form.UniqueWithSorted(arr3);
            Assert.AreEqual(num3, 5);
        }
        /// <summary>
        /// Edge case for testing with no elements , param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestSortedNoElem()
        {
            var form = new Form1();
            //no elements (edge case)
            int[] arr4 = [];
            int num4 = form.UniqueWithSorted(arr4);
            Assert.AreEqual(num4, 0);
        }
        /// <summary>
        /// Edge case for testing on reverse order, param: none, return: none, exception: none
        /// </summary>
        [Test]
        public void TestSortedReverse()
        {
            var form = new Form1();
            //no dupes + reverse sorted
            int[] arr5 = [5, 4, 3, 2, 1];
            int num5 = form.UniqueWithSorted(arr5);
            Assert.AreEqual(num5, 5);
        }

    }

}