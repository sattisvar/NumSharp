﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumSharp;
using NumSharp.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using FluentAssertions;
using NumSharp.UnitTest.Utilities;

namespace NumSharp.UnitTest.Selection
{
    [TestClass]
    public class IndexingTest : TestClass
    {
        [TestMethod]
        public void IndexAccessorGetter()
        {
            var nd = np.arange(12).reshape(3, 4);

            Assert.IsTrue(nd.GetInt32(1, 1) == 5);
            Assert.IsTrue(nd.GetInt32(2, 0) == 8);
        }

        [TestMethod]
        public void NDArrayAccess()
        {
            var nd = np.arange(4).reshape(2, 2);

            var row1 = (nd[0] as NDArray).MakeGeneric<int>();
            Assert.AreEqual(row1[0], 0);
            Assert.AreEqual(row1[1], 1);
        }

        [TestMethod]
        public void NDArrayAccess3Dim()
        {
            NDArray nd = np.arange(1, 19, 1).reshape(3, 3, 2);
            var row1 = (nd[0] as NDArray).MakeGeneric<int>();
            Assert.AreEqual(row1[0, 0], 1);
            Assert.AreEqual(row1[0, 1], 2);
            Assert.AreEqual(row1[1, 0], 3);
            Assert.AreEqual(row1[1, 1], 4);
            Assert.AreEqual(row1[2, 0], 5);
            Assert.AreEqual(row1[2, 1], 6);
        }

        [TestMethod]
        public void IndexAccessorSetter()
        {
            var nd = np.arange(12).reshape(3, 4);

            Assert.IsTrue(nd.GetInt32(0, 3) == 3);
            Assert.IsTrue(nd.GetInt32(1, 3) == 7);

            // set value
            nd.SetValue(10, 0, 0);
            Assert.IsTrue(nd.GetInt32(0, 0) == 10);
            Assert.IsTrue(nd.GetInt32(1, 3) == 7);
        }

        //TODO! NDArray[NDArray]
        //[TestMethod]
        //public void BoolArray()
        //{
        //    NDArray A = new double[] {1, 2, 3};

        //    NDArray booleanArr = new bool[] {false, false, true};

        //    A[booleanArr.MakeGeneric<bool>()] = 1;

        //    Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(A.ToArray<double>(), new double[] {1, 2, 1}));

        //    A = new double[,] {{1, 2, 3}, {4, 5, 6}};

        //    booleanArr = new bool[,] {{true, false, true}, {false, true, false}};

        //    A[booleanArr.MakeGeneric<bool>()] = -2;

        //    Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(A.ToArray<double>(), new double[] {-2, 2, -2, 4, -2, 6}));
        //}

        //TODO! NDArray[NDArray]
        //[TestMethod]
        //public void Compare()
        //{
        //    NDArray A = new double[,] {{1, 2, 3}, {4, 5, 6}};

        //    var boolArr = A < 3;
        //    Assert.IsTrue(Enumerable.SequenceEqual(boolArr.ToArray<bool>(), new[] {true, true, false, false, false, false}));

        //    A[A < 3] = -2;
        //    Assert.IsTrue(Enumerable.SequenceEqual(A.ToArray<double>(), new double[] {-2, -2, 3, 4, 5, 6}));

        //    var a = A[A == -2 | A > 5];

        //    Assert.IsTrue(Enumerable.SequenceEqual(a.ToArray<double>(), new double[] {-2, -2, 6}));
        //}

        [TestMethod]
        public void NDArrayByNDArray()
        {
            NDArray x = new double[] {1, 2, 3, 4, 5, 6};

            NDArray index = new int[] {1, 3, 5};

            NDArray selected = x[index];

            double[] a = (System.Array)selected as double[];
            double[] b = {2, 4, 6};

            Assert.IsTrue(Enumerable.SequenceEqual(a, b));
        }

        [TestMethod]
        public void Filter1D()
        {
            var nd = np.array(new int[] {3, 1, 1, 2, 3, 1});
            var filter = np.array(new int[] {0, 2, 5});
            var result = nd[filter];

            AssertAreEqual(new int[] {3, 1, 1}, result.ToArray<int>());
        }

        [TestMethod]
        public void Filter2D()
        {
            var nd = np.array(new int[][] {new int[] {3, 1, 1, 2}, new int[] {1, 2, 2, 3}, new int[] {2, 1, 1, 3},});
            var filter = np.array(new int[] {0, 2});
            var result = nd[filter];

            Assert.IsTrue(Enumerable.SequenceEqual(new int[] {3, 1, 1, 2}, (result[0] as NDArray).ToArray<int>()));
            Assert.IsTrue(Enumerable.SequenceEqual(new int[] {2, 1, 1, 3}, (result[1] as NDArray).ToArray<int>()));

            var x = nd[1];
            x.ravel();
        }

        [TestMethod]
        public void Slice1()
        {
            var x = np.arange(5);
            var y1 = x["1:3"];
            AssertAreEqual(y1.ToArray<int>(), new int[] {1, 2});

            var y2 = x["3:"];
            AssertAreEqual(y2.ToArray<int>(), new int[] {3, 4});
            y2[0] = 8;
            y2[1] = 9;
            Assert.AreEqual((int)y2[0], 8);
        }


        [TestMethod]
        public void Slice2()
        {
            //>>> x = np.arange(5)
            //        >>> x
            //array([0, 1, 2, 3, 4])
            //    >>> y = x[0:5]
            //    >>> y
            //array([0, 1, 2, 3, 4])
            var x = np.arange(5);
            var y1 = x["0:5"];
            AssertAreEqual(y1.ToArray<int>(), new int[] {0, 1, 2, 3, 4});
            y1 = x["1:4"];
            AssertAreEqual(y1.ToArray<int>(), new int[] {1, 2, 3});
            //    >>> z = x[:]
            //    >>> z
            //array([0, 1, 2, 3, 4])
            var y2 = x[":"];
            AssertAreEqual(y2.ToArray<int>(), new int[] {0, 1, 2, 3, 4});

            // out of bounds access is handled gracefully by numpy
            //    >>> y = x[0:77]
            //    >>> y
            //array([0, 1, 2, 3, 4])
            var y3 = x["0:77"];
            AssertAreEqual(y3.ToArray<int>(), new int[] {0, 1, 2, 3, 4});

            //    >>> y = x[-77:]
            //    >>> y
            //array([0, 1, 2, 3, 4])
            var y4 = x["-77:"];
            AssertAreEqual(y4.ToArray<int>(), new int[] {0, 1, 2, 3, 4});
            var y = x["-77:77"];
            AssertAreEqual(y.ToArray<int>(), new int[] {0, 1, 2, 3, 4});
        }

        [TestMethod]
        public void Slice3()
        {
            //>>> x = np.arange(6)
            //>>> x
            //array([0, 1, 2, 3, 4, 5])
            //>>> y = x[1:5]
            //>>> y
            //array([1, 2, 3, 4])
            //>>> z = y[:3]
            //>>> z
            //array([1, 2, 3])
            //>>> z[0] = 99
            //>>> y
            //array([99, 2, 3, 4])
            //>>> x
            //array([0, 99, 2, 3, 4, 5])
            //>>>
            var x = np.arange(6);
            var y = x["1:5"];
            AssertAreEqual(new int[] {1, 2, 3, 4,}, y.ToArray<int>());
            var z = y[":3"];
            AssertAreEqual(new int[] {1, 2, 3}, z.ToArray<int>());
            z[0] = 99;
            AssertAreEqual(new int[] {99, 2, 3, 4}, y.ToArray<int>());
            AssertAreEqual(new int[] {0, 99, 2, 3, 4, 5}, x.ToArray<int>());
        }

        [TestMethod]
        public void Slice4()
        {
            //>>> x = np.arange(5)
            //>>> x
            //array([0, 1, 2, 3, 4])
            var x = np.arange(5);
            //>>> y = x[2:4]
            //>>> y
            //array([2,3])
            var y = x["2:4"];
            Assert.AreEqual(2, (int)y[0]);
            Assert.AreEqual(3, (int)y[1]);
            y[0] = 77;
            y[1] = 99;
            Assert.AreEqual(77, (int)x[2]);
            Assert.AreEqual(99, (int)x[3]);
        }

        [TestMethod]
        public void Slice2x2Mul()
        {
            //>>> import numpy as np
            //>>> x = np.arange(4).reshape(2, 2)
            //>>> y = x[1:]
            //>>> x
            //array([[0, 1],
            //       [2, 3]])
            //>>> y
            //array([[2, 3]])
            //>>> y*=2
            //>>> y
            //array([[4, 6]])
            var x = np.arange(4).reshape(2, 2);
            var y = x["1:"]; // slice a row as 1D array
            y.Should().BeShaped(1, 2).And.BeOfValues(2, 3);
            var z = y * 2;
            z.Should().BeShaped(1, 2).And.BeOfValues(4, 6);
        }

        [TestMethod]
        public void Slice2x2Mul_2()
        {
            //>>> import numpy as np
            //>>> x = np.arange(4).reshape(2, 2)
            //>>> y = x[1:]
            //>>> x
            //array([[0, 1],
            //       [2, 3]])
            //>>> y
            //array([[2, 3]])
            //>>> y*=2
            //>>> y
            //array([[4, 6]])
            var x = np.arange(4).reshape(2, 2);
            var y = x["1"]; // slice a row as 1D array
            Assert.AreEqual(new Shape(2), new Shape(y.shape));
            AssertAreEqual(y.ToArray<int>(), new int[] {2, 3});
            y *= 2;
            AssertAreEqual(y.ToArray<int>(), new int[] {4, 6});
            //AssertAreEqual(x.ToArray<int>(), new int[] { 0, 1, 4, 6 });
        }

        [TestMethod]
        public void Slice2x2Mul_3()
        {
            var x = np.arange(4).reshape(2, 2);
            var y = x[":,1"]; // slice a column as 1D array (shape 2)
            y.Should().BeShaped(2).And.BeOfValues(1, 3);
            var z = y * 2;
            z.Should().BeShaped(2).And.BeOfValues(2, 6);
        }

        [TestMethod]
        public void Slice2x2Mul_4()
        {
            var x = np.arange(4).reshape(2, 2);
            var y = x[":,1"]; // slice a column as 1D array (shape 2)
            y.Should().BeShaped(2).And.BeOfValues(1, 3);
            var z = 2 * y;
            z.Should().BeShaped(2).And.BeOfValues(2, 6);
        }

        [TestMethod]
        public void Slice2x2Mul_5()
        {
            var x = np.arange(4).reshape(2, 2);
            var y = x[":,1"]; // slice a column as 1D array (shape 2)
            y.Should().BeShaped(2).And.BeOfValues(1, 3);
            var z = y * y;
            z.Should().BeShaped(2).And.BeOfValues(1, 9);
        }

        [TestMethod]
        public void Slice2x2Mul_6()
        {
            var x = np.arange(4).reshape(2, 2);
            x.Should().BeShaped(2, 2).And.BeOfValues(0, 1, 2, 3);
            var z = x * x;
            z.Should().BeShaped(2, 2).And.BeOfValues(0, 1, 4, 9);
        }

        [Ignore("This can never work because C# doesn't allow overloading of the assignment operator")]
        [TestMethod]
        public void Slice2x2Mul_AssignmentChangesOriginal()
        {
            //>>> import numpy as np
            //>>> x = np.arange(4).reshape(2, 2)
            //>>> y = x[1:]
            //>>> x
            //array([[0, 1],
            //       [2, 3]])
            //>>> y
            //array([[2, 3]])
            //>>> y*=2
            //>>> y
            //array([[4, 6]])
            //>>> x
            //array([[0, 1],
            //       [4, 6]])
            var x = np.arange(4).reshape(2, 2);
            var y = x["1"]; // slice a row as 1D array
            Assert.AreEqual(new Shape(2), new Shape(y.shape));
            AssertAreEqual(y.ToArray<int>(), new int[] {2, 3});
            y *= 2;
            AssertAreEqual(y.ToArray<int>(), new int[] {4, 6});
            AssertAreEqual(x.ToArray<int>(), new int[] {0, 1, 4, 6}); // <------- this fails because in C# we can not intercept assignment to a variable
        }

        [TestMethod]
        public void Slice5()
        {
            var x = np.arange(6).reshape(3, 2);
            var y = x[":,0"];
            AssertAreEqual(new int[] {0, 2, 4,}, y.ToArray<int>());
            var z = x["1,:"];
            AssertAreEqual(new int[] {2, 3}, z.ToArray<int>());
            z[0] = 99;
            AssertAreEqual(new int[] {99, 3}, z.ToArray<int>());
            AssertAreEqual(new int[] {0, 99, 4}, y.ToArray<int>());
            AssertAreEqual(new int[] {0, 1, 99, 3, 4, 5}, x.ToArray<int>());
        }

        [TestMethod]
        public void Slice_Step()
        {
            //>>> x = np.arange(5)
            //>>> x
            //array([0, 1, 2, 3, 4])
            var x = np.arange(5);
            //>>> y = x[::-1]
            //>>> y
            //array([4, 3, 2, 1, 0])
            var y = x["::-1"];
            AssertAreEqual(y.ToArray<int>(), new int[] {4, 3, 2, 1, 0});

            //>>> y = x[::2]
            //>>> y
            //array([0, 2, 4])
            y = x["::2"];
            AssertAreEqual(y.ToArray<int>(), new int[] {0, 2, 4});
        }

        [TestMethod]
        public void Slice_Step1()
        {
            //>>> x = np.arange(6)
            //>>> x
            //array([0, 1, 2, 3, 4, 5])
            //>>> y = x[::- 1]
            //>>> y
            //array([5, 4, 3, 2, 1, 0])
            //>>> y[0] = 99
            //>>> x
            //array([0, 1, 2, 3, 4, 99])
            //>>> y
            //array([99, 4, 3, 2, 1, 0])
            //>>> y = x[::-1]
            //>>> y
            //array([5, 4, 3, 2, 1, 0])
            var x = np.arange(6);
            var y = x["::-1"];
            y[0] = 99;
            AssertAreEqual(new int[] {0, 1, 2, 3, 4, 99}, x.ToArray<int>());
            AssertAreEqual(new int[] {99, 4, 3, 2, 1, 0}, y.ToArray<int>());
            //>>> z = y[::2]
            //>>> z
            //array([99, 3, 1])
            //>>> z[1] = 111
            //>>> x
            //array([0, 1, 2, 111, 4, 99])
            //>>> y
            //array([99, 4, 111, 2, 1, 0])
            var z = y["::2"];
            AssertAreEqual(new int[] {99, 3, 1}, z.ToArray<int>());
            z[1] = 111;
            AssertAreEqual(new int[] {99, 111, 1}, (int[])z);
            AssertAreEqual(new int[] {0, 1, 2, 111, 4, 99}, x.ToArray<int>());
            AssertAreEqual(new int[] {99, 4, 111, 2, 1, 0}, y.ToArray<int>());
        }

        [TestMethod]
        public void Slice_Step2()
        {
            //>>> x = np.arange(5)
            //>>> x
            //array([0, 1, 2, 3, 4])
            var x = np.arange(5);
            //>>> y = x[::2]
            //>>> y
            //array([0, 2, 4])
            var y = x["::2"];
            Assert.AreEqual(0, (int)y[0]);
            Assert.AreEqual(2, (int)y[1]);
            Assert.AreEqual(4, (int)y[2]);
        }

        [TestMethod]
        public void Slice_Step3()
        {
            var x = np.arange(5);
            Assert.AreEqual("[0, 1, 2, 3, 4]", x.ToString());
            var y = x["::2"];
            Assert.AreEqual("[0, 2, 4]", y.ToString());
        }

        [TestMethod]
        public void Slice_Step_With_Offset()
        {
            //>>> x = np.arange(9).astype(np.uint8)
            //>>> x
            //array([0, 1, 2, 3, 4, 5, 6, 7, 8])
            var x = np.arange(9).astype(np.uint8);

            //>>> y = x[::3]
            //>>> y
            //array([0, 3, 6], dtype=uint8)
            var y0 = x["::3"];
            AssertAreEqual(new byte[] {0, 3, 6}, y0.ToArray<byte>());

            //>>> y = x[1::3]
            //>>> y
            //array([1, 4, 7], dtype=uint8)
            var y1 = x["1::3"];
            AssertAreEqual(new byte[] {1, 4, 7}, y1.ToArray<byte>());

            //>>> y = x[2::3]
            //>>> y
            //array([2, 5, 8], dtype=uint8)
            var y2 = x["2::3"];
            AssertAreEqual(new byte[] {2, 5, 8}, y2.ToArray<byte>());

            //>>> y = x[3::3]
            //>>> y
            //array([3, 6], dtype=uint8)
            var y3 = x["3::3"];
            AssertAreEqual(new byte[] {3, 6}, y3.ToArray<byte>());
        }


        [TestMethod]
        public void Slice3x2x2()
        {
            //>>> x = np.arange(12).reshape(3, 2, 2)
            //>>> x
            //array([[[0, 1],
            //        [ 2,  3]],
            //
            //       [[ 4,  5],
            //        [ 6,  7]],
            //
            //       [[ 8,  9],
            //        [10, 11]]])
            //>>> y1 = x[1:]
            //>>> y1
            //array([[[ 4,  5],
            //        [ 6,  7]],
            //
            //       [[ 8,  9],
            //        [10, 11]]])

            var x = np.arange(12).reshape(3, 2, 2);
            var y1 = x["1:"];
            Assert.IsTrue(Enumerable.SequenceEqual(y1.shape, new int[] {2, 2, 2}));
            Assert.IsTrue(Enumerable.SequenceEqual(y1.ToArray<int>(), new int[] {4, 5, 6, 7, 8, 9, 10, 11}));
            Assert.IsTrue(Enumerable.SequenceEqual(y1[0, 1].ToArray<int>(), new int[] {6, 7}));

            var y1_0 = y1[0];
            Assert.IsTrue(Enumerable.SequenceEqual(y1_0.shape, new int[] {2, 2}));
            Assert.IsTrue(Enumerable.SequenceEqual(y1_0.ToArray<int>(), new int[] {4, 5, 6, 7}));

            // change view
            y1[0, 1] = new int[] {100, 101};
            Assert.IsTrue(Enumerable.SequenceEqual(x.ToArray<int>(), new int[] {0, 1, 2, 3, 4, 5, 100, 101, 8, 9, 10, 11}));
            Assert.IsTrue(Enumerable.SequenceEqual(y1.ToArray<int>(), new int[] {4, 5, 100, 101, 8, 9, 10, 11}));

            var y2 = x["2:"];
            Assert.IsTrue(Enumerable.SequenceEqual(y2.shape, new int[] {1, 2, 2}));
            Assert.IsTrue(Enumerable.SequenceEqual(y2.ToArray<int>(), new int[] {8, 9, 10, 11}));
        }

        [TestMethod]
        public void AssignGeneric1DSlice1()
        {
            //>>> x = np.arange(5)
            //>>> y1 = np.arange(5, 8)
            //>>> y2 = np.arange(10, 13)
            //>>> x
            //array([0, 1, 2, 3, 4])
            //>>>
            //>>> xS1 = x[1:4]
            //>>> xS1[0] = y1[0]
            //>>> xS1[1] = y1[1]
            //>>> xS1[2] = y1[2]
            //>>>
            //>>> xS1
            //array([5, 6, 7])
            //>>> x
            //array([0, 5, 6, 7, 4])
            //>>>
            //>>> xS2 = x[1:-1]
            //>>> xS2[:] = y2
            //>>>
            //>>> xS2
            //array([10, 11, 12])
            //>>> x
            //array([0, 10, 11, 12, 4])
            //>>>

            var x = np.arange(5).MakeGeneric<int>();
            var y1 = np.arange(5, 8).MakeGeneric<int>();
            var y2 = np.arange(10, 13).MakeGeneric<int>();

            AssertAreEqual(new int[] {0, 1, 2, 3, 4}, x.ToArray<int>());

            var xS1 = x["1:4"];
            xS1[0] = y1[0];
            xS1[1] = y1[1];
            xS1[2] = y1[2];

            AssertAreEqual(new int[] {5, 6, 7}, xS1.ToArray<int>());
            AssertAreEqual(new int[] {0, 5, 6, 7, 4}, x.ToArray<int>());

            var xS2 = x[new Slice(1, -1)];
            xS2[":"] = y2;

            AssertAreEqual(new int[] {10, 11, 12}, xS2.ToArray<int>());
            AssertAreEqual(new int[] {0, 10, 11, 12, 4}, x.ToArray<int>());
        }

        [TestMethod]
        public void AssignGeneric1DSliceWithStepAndOffset1()
        {
            //>>> x = np.arange(9).astype(np.uint16)
            //>>> x
            //array([0, 1, 2, 3, 4, 5, 6, 7, 8], dtype = uint16)
            var x = np.arange(9).astype(np.uint16).MakeGeneric<ushort>();

            //>>> yS1 = np.arange(10, 13).astype(np.uint16)
            //>>> yS1
            //array([10, 11, 12], dtype = uint16)
            var yS0 = np.array<ushort>(new ushort[] {10, 11, 12}).MakeGeneric<ushort>();

            //>>> y0 = x[::3]
            //>>> y0
            //array([0, 3, 6], dtype = uint16)
            var y0 = x["::3"];
            AssertAreEqual(new ushort[] {0, 3, 6}, y0.ToArray<ushort>());

            //>>> x[::3] = yS0
            //>>> y0
            //array([10, 11, 12], dtype = uint16)
            x["::3"] = yS0;
            AssertAreEqual(new ushort[] {10, 11, 12}, y0.ToArray<ushort>());
            //>>> x
            //array([10, 1, 2, 11, 4, 5, 12, 7, 8], dtype = uint16)
            AssertAreEqual(new ushort[] {10, 1, 2, 11, 4, 5, 12, 7, 8}, x.ToArray<ushort>());

            //>>> x[1::3] = yS
            //>>> x
            //array([10, 10, 2, 11, 11, 5, 12, 12, 8], dtype = uint16)
            x["1::3"] = yS0;
            AssertAreEqual(new ushort[] {10, 10, 2, 11, 11, 5, 12, 12, 8}, x.ToArray<ushort>());
        }

        [TestMethod]
        public void AssignGeneric2DSlice1()
        {
            //>>> x = np.arange(9).reshape(3, 3)
            //>>> y1 = np.arange(6, 9)
            //>>> y2 = np.arange(12, 15)
            //>>>
            //>>> x
            //array([[0, 1, 2],
            //       [3, 4, 5],
            //       [6, 7, 8]])
            //>>>
            //>>> xS1 = x[1]
            //>>> xS1[0] = y1[0]
            //>>> xS1[1] = y1[1]
            //>>> xS1[2] = y1[2]
            //>>>
            //>>> xS1
            //array([6, 7, 8])
            //>>> x
            //array([[0, 1, 2],
            //       [6, 7, 8],
            //       [6, 7, 8]])
            //>>>
            //>>> xS2 = x[1:-1]
            //>>> xS2[:] = y2
            //>>>
            //>>> xS2
            //array([[12, 13, 14]])
            //>>> x
            //array([[ 0,  1,  2],
            //       [12, 13, 14],
            //       [ 6,  7,  8]])

            var x = np.arange(9).reshape(3, 3).MakeGeneric<int>();
            var y1 = np.arange(6, 9).MakeGeneric<int>();
            var y2 = np.arange(12, 15).MakeGeneric<int>();

            AssertAreEqual(new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8}, x.ToArray<int>());

            var xS1 = x["1"];
            xS1[0] = y1[0];
            xS1[1] = y1[1];
            xS1[2] = y1[2];

            AssertAreEqual(new int[] {6, 7, 8}, xS1.ToArray<int>());
            AssertAreEqual(new int[] {0, 1, 2, 6, 7, 8, 6, 7, 8}, x.ToArray<int>());

            var xS2 = x[new Slice(1, -1)];
            xS2[":"] = y2;

            AssertAreEqual(new int[] {12, 13, 14}, xS2.ToArray<int>());
            AssertAreEqual(new int[] {0, 1, 2, 12, 13, 14, 6, 7, 8}, x.ToArray<int>());
        }

        [TestMethod]
        public void Transpose10x10()
        {
            new Action(() =>
            {
                var array = np.arange(100).reshape(3, 3, 3);
                for (var i = 0; i < array.shape[0]; i++)
                {
                    for (var j = 0; j < array.shape[1]; j++)
                    {
                        Console.WriteLine(array[i, j].ToString());
                    }
                }
            }).Should().NotThrow("It has to run completely.");
        }

        /// <summary>
        /// Based on issue https://github.com/SciSharp/NumSharp/issues/293
        /// </summary>
        [TestMethod]
        public void CastingWhenSettingDifferentType()
        {
            NDArray output = np.zeros(5);
            double newValDouble = 2;
            int newValInt = 4;
            output[3] = newValDouble; // This works fine
            new Action(() => output[4] = newValInt).Should().NotThrow<NullReferenceException>(); // throws System.NullReferenceException
            output.Array.GetIndex(4).Should().Be(newValInt);
        }

        private static NDArray x = np.arange(10, 1, -1);
        private static NDArray y = np.arange(35).reshape(5, 7);

        [TestMethod]
        public void IndexNDArray_Case1()
        {
            x[np.array(new int[] {3, 3, 1, 8})].Array.Should().ContainInOrder(7, 7, 9, 2);
        }

        [TestMethod]
        public void IndexNDArray_Case2_NegativeIndex()
        {
            x[np.array(new int[] {3, 3, -3, 8})].Array.Should().ContainInOrder(7, 7, 4, 2);
        }

        [TestMethod]
        public void IndexNDArray_Case3()
        {
            new Action(() =>
            {
                var a = x[np.array(new int[] {3, 3, 20, 8})];
            }).Should().Throw<IndexOutOfRangeException>();
        }

        [TestMethod]
        public void IndexNDArray_Case4_Shaped()
        {
            var ret = x[np.array(new int[][] {new int[] {1, 1}, new int[] {2, 3},})];
            ret.Array.Should().ContainInOrder(9, 9, 8, 7);
            ret.shape.Should().ContainInOrder(2, 2);
        }

        [TestMethod]
        public void IndexNDArray_Case5_Shaped()
        {
            var ret = np.arange(0, 10).reshape(2, 5)[np.array(new int[][] {new int[] {0, 1}, new int[] {1, 3},})];
            Console.WriteLine((string)ret);
            ret.Array.Cast<int>().Should().ContainInOrder(0, 1, 1, 3);
            ret.shape.Should().ContainInOrder(2, 2);
        }

        [TestMethod]
        public void IndexNDArray_Case6_Shaped()
        {
            var ret = np.arange(0, 10).reshape(2, 5)[np.array(new int[] {0, 1, 1})];
            Console.WriteLine((string)ret);
            ret.shape.Should().ContainInOrder(3, 5);
            ret[1, 0].GetValue(0).Should().Be(5);
            ret[1, 4].GetValue(0).Should().Be(9);
            ret[2, 0].GetValue(0).Should().Be(5);
            ret[2, 4].GetValue(0).Should().Be(9);
        }

        [TestMethod]
        public void IndexNDArray_Case7_Multi()
        {
            var ret = y[np.array(new int[] {0, 2, 4}), np.array(new int[] {0, 1, 2})];
            ret.Array.Should().ContainInOrder(0, 15, 30);
            ret.shape.Should().ContainInOrder(3);
        }

        [TestMethod]
        public void IndexNDArray_Case8_Multi()
        {
            var a = np.arange(27).reshape(3, 3, 3) + 1;
            var x = np.repeat(np.arange(3), 3);
            var ret = a[x, x, x];
            ret.Array.Should().ContainInOrder(1, 14, 27);
            ret.shape.Should().ContainInOrder(9);
        }

        [TestMethod]
        public void IndexNDArray_Case14_Multi_Slice()
        {
            var a = np.arange(27 * 2).reshape(2, 3, 3, 3) + 1;
            var x = np.repeat(np.arange(3), 3);
            var ret = a["0,:"][x, x, x];
            ret.Array.Should().ContainInOrder(1, 14, 27);
            ret.shape.Should().ContainInOrder(9);
        }

        [TestMethod]
        public void IndexNDArray_Case9_Multi()
        {
            var a = np.arange(27).reshape(3, 3, 3) + 1;
            var x = np.repeat(np.arange(3), 3 * 2).reshape(3, 3, 2);
            var ret = a[x, x, x];
            Console.WriteLine((string)ret);
            Console.WriteLine(ret.Shape);
            ret.Array.Should().ContainInOrder(np.repeat(np.array(1, 14, 27), 6).flat.ToArray<int>());
            ret.shape.Should().ContainInOrder(3, 3, 2);
        }

        [TestMethod]
        public void IndexNDArray_Case10_Multi()
        {
            new Action(() =>
            {
                var ret = y[np.array(new int[] {0, 2, 4}), np.array(new int[] {0, 1})];
            }).Should().Throw<IncorrectShapeException>();
        }


        [TestMethod]
        public void IndexNDArray_Case11_Multi()
        {
            var ret = y[np.array(new int[] {0, 2, 4})];
            ret.shape.Should().ContainInOrder(3, 7);
            ret[0].Array.Should().ContainInOrder(0, 1, 2, 3, 4, 5, 6);
            ret[1].Array.Should().ContainInOrder(14, 15, 16, 17, 18, 19, 20);
            ret[2].Array.Should().ContainInOrder(28, 29, 30, 31, 32, 33, 34);
        }

        [TestMethod]
        public void IndexNDArray_Case12_Multi()
        {
            var a = np.arange(27).reshape(1, 3, 3, 1, 3);
            Console.WriteLine((string)a);
            Console.WriteLine(a.Shape);
            var ret = a[new int[] {0, 0, 0}, new int[] {0, 1, 2}];
            ret.shape.Should().ContainInOrder(3, 3, 1, 3);
            ret.GetValue(2, 0, 0, 2).Should().Be(20);
            //ret[0, 0].Array.Should().ContainInOrder(0, 1, 2, 3, 4, 5, 6);
            //ret[1, 0].Array.Should().ContainInOrder(14, 15, 16, 17, 18, 19, 20);
            //ret[2, 0].Array.Should().ContainInOrder(28, 29, 30, 31, 32, 33, 34);
        }

        [TestMethod]
        public void IndexNDArray_Case13_Multi()
        {
            var ret = y[np.array(new int[] {0, 2, 4})];
            ret.shape.Should().ContainInOrder(3, 7);
            ret[0].Array.Should().ContainInOrder(0, 1, 2, 3, 4, 5, 6);
            ret[1].Array.Should().ContainInOrder(14, 15, 16, 17, 18, 19, 20);
            ret[2].Array.Should().ContainInOrder(28, 29, 30, 31, 32, 33, 34);
        }

        [TestMethod]
        public void Slice_TwoMinusOne()
        {
            var a = np.arange(1 * 1 * 3).reshape((1, 1, 3)); //0, 1
            var b = np.arange(1 * 3 * 1).reshape((1, 3, 1)); //0, 1
            b = b["-1, -1"]; //2, 0
            a = a["-1, -1"]; //0, 0

            a.Should().BeOfValues(0, 1, 2).And.BeShaped(3);
            b.Should().BeOfValues(2).And.BeShaped(1);
        }

        [TestMethod]
        public void IndexNegativeCoordiantes()
        {
            var p = np.arange(6).reshape(2, 3);
            p[0, -1].Should().BeScalar(2);
            p[-1, 0].Should().BeScalar(3);
            p[-1, 1].Should().BeScalar(4);
        }

        [TestMethod]
        public void MinusOne_Case1()
        {
            var a = np.arange(4 * 1 * 10 * 1).reshape((4, 1, 10, 1))[-1];
            a.Should().BeOfValues(30, 31, 32, 33, 34, 35, 36, 37, 38, 39);
        }

        [TestMethod]
        public void MinusOne_Case2()
        {
            var a = np.arange(4 * 1 * 10 * 1).reshape((4, 1, 10, 1))["-1"];
            a.Should().BeOfValues(30, 31, 32, 33, 34, 35, 36, 37, 38, 39);
        }

        [TestMethod]
        public void MinusOne_Case3()
        {
            var a = np.arange(4 * 1 * 10 * 1).reshape((4, 1, 10, 1))[-1][-1];
            a.Should().BeOfValues(30, 31, 32, 33, 34, 35, 36, 37, 38, 39);
        }

        [TestMethod]
        public void MinusOne_Case4()
        {
            var a = np.arange(4 * 1 * 10 * 1).reshape((4, 1, 10, 1))["-1"]["-1"];
            a.Should().BeOfValues(30, 31, 32, 33, 34, 35, 36, 37, 38, 39);
        }

        [TestMethod]
        public void Broadcasted_Case1()
        {
            var a = np.arange(1 * 1 * 3).reshape((1, 1, 3)); //0, 1
            var b = np.arange(1 * 3 * 1).reshape((1, 3, 1)); //0, 1
            var (c, d) = np.broadcast_arrays(a, b);
            c.Should().BeOfValues(0, 1, 2, 0, 1, 2, 0, 1, 2);
            d.Should().BeOfValues(0, 0, 0, 1, 1, 1, 2, 2, 2);
            d = d["-1, -1"]; //2, 0
            c = c["-1, -1"]; //0, 0
            c.Should().BeOfValues(0, 1, 2).And.BeShaped(3);
            d.Should().BeOfValues(2, 2, 2).And.BeShaped(3);
        }

        [TestMethod]
        public void Broadcasted_Case2()
        {
            var a = np.arange(1 * 1 * 3).reshape((1, 1, 3));
            var b = np.arange(1 * 3 * 3).reshape((1, 3, 3));
            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeOfValues(0, 1, 2, 0, 1, 2, 0, 1, 2).And.BeShaped(1, 3, 3);
            b.Should().BeOfValues(0, 1, 2, 3, 4, 5, 6, 7, 8).And.BeShaped(1, 3, 3);
            b = b["-1, -1"];
            a = a["-1, -1"];
            a.Should().BeOfValues(0, 1, 2).And.BeShaped(3);
            b.Should().BeOfValues(6, 7, 8).And.BeShaped(3);
        }

        [TestMethod]
        public void Broadcasted_Case3()
        {
            var a = np.arange(2 * 1 * 3).reshape((2, 1, 3));
            var b = np.arange(2 * 3 * 3).reshape((2, 3, 3));
            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeOfValues(0, 1, 2, 0, 1, 2, 0, 1, 2, 3, 4, 5, 3, 4, 5, 3, 4, 5).And.BeShaped(2, 3, 3);
            b.Should().BeOfValues(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17).And.BeShaped(2, 3, 3);
            a = a["-1"];
            b = b["-1"];
            a.Should().BeOfValues(3, 4, 5, 3, 4, 5, 3, 4, 5).And.BeShaped(3, 3);
            b.Should().BeOfValues(9, 10, 11, 12, 13, 14, 15, 16, 17).And.BeShaped(3, 3);
        }

        [TestMethod]
        public void Broadcasted_Case4()
        {
            var a = np.arange(2 * 10 * 3).reshape((2, 10, 3));
            var b = np.arange(2 * 1 * 3).reshape((2, 1, 3));
            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeShaped(2, 10, 3);
            b.Should().BeShaped(2, 10, 3);
            Console.WriteLine(a.Shape);
            a = a["-1"];
            b = b["-1"];
            a.Should().BeShaped(10, 3);
            b.Should().BeShaped(10, 3);
        }

        [TestMethod]
        public void Broadcasted_Case5()
        {
            var a = np.arange(2 * 1 * 3).reshape((2, 1, 3)); //0, 1
            var b = np.arange(2 * 3 * 3).reshape((2, 3, 3)); //0, 1

            a = a["-1"];
            b = b["-1"];
            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeOfValues(3, 4, 5, 3, 4, 5, 3, 4, 5).And.BeShaped(3, 3);
            b.Should().BeOfValues(9, 10, 11, 12, 13, 14, 15, 16, 17).And.BeShaped(3, 3);
            Console.WriteLine(a.Shape);
            Console.WriteLine(a.ToString());
            Console.WriteLine(b.ToString());
        }


        [TestMethod]
        public void Broadcasted_Case6_GetData()
        {
            var a = np.arange(3 * 1 * 2 * 2).reshape((3, 1, 2, 2));
            var b = np.arange(3 * 2 * 2).reshape((3, 2, 2));

            Console.WriteLine(b.Shape.strides.ToString(false));
            (a, b) = np.broadcast_arrays(a, b);
            Console.WriteLine(b.Shape.strides.ToString(false));
            a.Should().BeShaped(3, 3, 2, 2);
            b.Should().BeShaped(3, 3, 2, 2);

            var ret = b[0, 1];
            ret.Should().BeShaped(2, 2).And.BeOfValues(4, 5, 6, 7);
        }

        [TestMethod]
        public void Broadcasted_Case7_GetData()
        {
            var a = np.arange(2 * 3 * 2 * 2).reshape((2, 3, 2, 2));
            var b = np.arange(2 * 1 * 2 * 2).reshape((2, 1, 2, 2))[0, Slice.All];

            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeShaped(2, 3, 2, 2);
            b.Should().BeShaped(2, 3, 2, 2);

            var ret = b[1, 2];
            ret.Should().BeShaped(2, 2).And.BeOfValues(0, 1, 2, 3);
        }

        [TestMethod]
        public void Broadcasted_Case8_GetData()
        {
            var a = np.arange(2 * 3 * 2 * 2).reshape((2, 3, 2, 2));
            var b = np.arange(2 * 2 * 1 * 2 * 2).reshape((2, 2, 1, 2, 2))[0, 1, Slice.All];
            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeShaped(2, 3, 2, 2);
            b.Should().BeShaped(2, 3, 2, 2);

            var ret = b[1, 2];
            ret.Should().BeShaped(2, 2).And.BeOfValues(4, 5, 6, 7);
        }

        [TestMethod]
        public void Broadcasted_Case9()
        {
            var a = np.arange(2 * 3 * 2 * 2).reshape((2, 3, 2, 2));
            var b = np.arange(2 * 2 * 1 * 2 * 2).reshape((2, 2, 1, 2, 2))[0, 1, Slice.All];
            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeShaped(2, 3, 2, 2);
            b.Should().BeShaped(2, 3, 2, 2);

            var ret = b[1, 2];
            var str = ret.ToString(true);
            Console.WriteLine(str);
            str.Should().Be(np.array(4, 5, 6, 7).reshape(2, 2).ToString(true));
        }

        [TestMethod]
        public void Slice_MinusOne()
        {
            var a = np.arange(4 * 1 * 1 * 1).reshape(4, 1, 1, 1);
            a["-1, :"].Should().Be(a["3, :"]);
        }

        [TestMethod]
        public void Broadcasted_Case9_Sliced()
        {
            var a = np.arange(4 * 1 * 1 * 1).reshape(4, 1, 1, 1)["3, :"];
            var b = np.arange(4 * 1 * 10 * 1).reshape(4, 1, 10, 1)["3, :"];

            (a, b) = np.broadcast_arrays(a, b);

            a.Should().BeBroadcasted().And.BeShaped(1, 10, 1);
            b.Should().BeBroadcasted().And.BeShaped(1, 10, 1);
            a.Should().AllValuesBe(3);
            b.Should().BeOfValues(30, 31, 32, 33, 34, 35, 36, 37, 38, 39);
        }

        [TestMethod]
        public void Broadcasted_Case10_Sliced()
        {
            var a = np.arange(2 * 2 * 1 * 3).reshape((2, 2, 1, 3))["0, -1"]; //0, 1
            var b = np.arange(2 * 2 * 3 * 3).reshape((2, 2, 3, 3))["0, -1"]; //0, 1

            (a, b) = np.broadcast_arrays(a, b);
            a.Should().BeOfValues(3, 4, 5, 3, 4, 5, 3, 4, 5).And.BeShaped(3, 3);
            b.Should().BeOfValues(9, 10, 11, 12, 13, 14, 15, 16, 17).And.BeShaped(3, 3);
            Console.WriteLine(a.Shape);
            Console.WriteLine(a.ToString());
            Console.WriteLine(b.ToString());
        }

        [TestMethod]
        public void SliceEndingWithAll()
        {
            var a = np.arange(9).reshape(3, 3);
            
            //its supposed to be a memory slice because 
            var sliced = a[-1, Slice.All];
            sliced.Should().BeShaped(3).And.NotBeSliced();
        }
    }
}
