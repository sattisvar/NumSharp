﻿using System;
using NumSharp.Backends;
using NumSharp.Utilities;

namespace NumSharp
{
    public static partial class np
    {
        /// <summary>
        ///     Return evenly spaced numbers over a specified interval.<br></br>
        ///     Returns num evenly spaced samples, calculated over the interval[start, stop].<br></br>
        ///     The endpoint of the interval can optionally be excluded.
        /// </summary>
        /// <param name="start">The starting value of the sequence.</param>
        /// <param name="stop">The end value of the sequence, unless endpoint is set to False. In that case, the sequence consists of all but the last of num + 1 evenly spaced samples, so that stop is excluded. Note that the step size changes when endpoint is False.</param>
        /// <param name="num">Number of samples to generate. Default is 50. Must be non-negative.</param>
        /// <param name="endpoint">If True, stop is the last sample. Otherwise, it is not included. Default is True.</param>
        /// <param name="dtype">The type of the output array. If dtype is not given, infer the data type from the other input arguments.</param>
        /// <remarks>https://docs.scipy.org/doc/numpy-1.15.0/reference/generated/numpy.linspace.html</remarks>
        public static NDArray linspace(double start, double stop, int num, bool endpoint, Type dtype)
        {
            return linspace(start, stop, num, endpoint, (dtype ?? typeof(double)).GetTypeCode());
        }

        /// <summary>
        ///     Return evenly spaced numbers over a specified interval.<br></br>
        ///     Returns num evenly spaced samples, calculated over the interval[start, stop].<br></br>
        ///     The endpoint of the interval can optionally be excluded.
        /// </summary>
        /// <param name="start">The starting value of the sequence.</param>
        /// <param name="stop">The end value of the sequence, unless endpoint is set to False. In that case, the sequence consists of all but the last of num + 1 evenly spaced samples, so that stop is excluded. Note that the step size changes when endpoint is False.</param>
        /// <param name="num">Number of samples to generate. Default is 50. Must be non-negative.</param>
        /// <param name="endpoint">If True, stop is the last sample. Otherwise, it is not included. Default is True.</param>
        /// <param name="dtype">The type of the output array. If dtype is not given, infer the data type from the other input arguments.</param>
        /// <remarks>https://docs.scipy.org/doc/numpy-1.15.0/reference/generated/numpy.linspace.html</remarks>
        public static NDArray linspace(float start, float stop, int num, bool endpoint, Type dtype)
        {
            return linspace(start, stop, num, endpoint, (dtype ?? typeof(float)).GetTypeCode());
        }

        /// <summary>
        ///     Return evenly spaced numbers over a specified interval.<br></br>
        ///     Returns num evenly spaced samples, calculated over the interval[start, stop].<br></br>
        ///     The endpoint of the interval can optionally be excluded.
        /// </summary>
        /// <param name="start">The starting value of the sequence.</param>
        /// <param name="stop">The end value of the sequence, unless endpoint is set to False. In that case, the sequence consists of all but the last of num + 1 evenly spaced samples, so that stop is excluded. Note that the step size changes when endpoint is False.</param>
        /// <param name="num">Number of samples to generate. Default is 50. Must be non-negative.</param>
        /// <param name="endpoint">If True, stop is the last sample. Otherwise, it is not included. Default is True.</param>
        /// <param name="typeCode">The type of the output array. If dtype is not given, infer the data type from the other input arguments.</param>
        /// <remarks>https://docs.scipy.org/doc/numpy-1.15.0/reference/generated/numpy.linspace.html</remarks>
        public static NDArray linspace(double start, double stop, int num, bool endpoint = true, NPTypeCode typeCode = NPTypeCode.Double)
        {
            if (typeCode == NPTypeCode.Empty)
                throw new ArgumentException("Invalid typeCode", nameof(typeCode));

            NDArray ret = new NDArray(typeCode, new Shape(num), false);
            double step = (stop - start) / (endpoint ? num - 1.0 : num);

            switch (ret.GetTypeCode)
            {
#if _REGEN
	            case NPTypeCode.Boolean:
	            {
                    unsafe
                    {
                        var addr = (bool*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = (start + i * step) != 0;
                    }

                    return ret;
	            }
	            %foreach except(supported_dtypes, "Boolean"),except(supported_dtypes_lowercase, "bool")%
	            case NPTypeCode.#1:
	            {
                    unsafe
                    {
                        var addr = (#2*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.To#1(start + i * step);
                    }

                    return ret;
	            }
	            %
	            default:
		            throw new NotSupportedException();
#else
                case NPTypeCode.Boolean:
                {
                    unsafe
                    {
                        var addr = (bool*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = start + i * step != 0;
                    }

                    return ret;
                }

                case NPTypeCode.Byte:
                {
                    unsafe
                    {
                        var addr = (byte*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToByte(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Int16:
                {
                    unsafe
                    {
                        var addr = (short*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToInt16(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.UInt16:
                {
                    unsafe
                    {
                        var addr = (ushort*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToUInt16(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Int32:
                {
                    unsafe
                    {
                        var addr = (int*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToInt32(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.UInt32:
                {
                    unsafe
                    {
                        var addr = (uint*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToUInt32(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Int64:
                {
                    unsafe
                    {
                        var addr = (long*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToInt64(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.UInt64:
                {
                    unsafe
                    {
                        var addr = (ulong*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToUInt64(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Char:
                {
                    unsafe
                    {
                        var addr = (char*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToChar(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Double:
                {
                    unsafe
                    {
                        var addr = (double*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToDouble(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Single:
                {
                    unsafe
                    {
                        var addr = (float*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToSingle(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Decimal:
                {
                    unsafe
                    {
                        var addr = (decimal*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToDecimal(start + i * step);
                    }

                    return ret;
                }

                default:
                    throw new NotSupportedException();
#endif
            }
        }

        /// <summary>
        ///     Return evenly spaced numbers over a specified interval.<br></br>
        ///     Returns num evenly spaced samples, calculated over the interval[start, stop].<br></br>
        ///     The endpoint of the interval can optionally be excluded.
        /// </summary>
        /// <param name="start">The starting value of the sequence.</param>
        /// <param name="stop">The end value of the sequence, unless endpoint is set to False. In that case, the sequence consists of all but the last of num + 1 evenly spaced samples, so that stop is excluded. Note that the step size changes when endpoint is False.</param>
        /// <param name="num">Number of samples to generate. Default is 50. Must be non-negative.</param>
        /// <param name="endpoint">If True, stop is the last sample. Otherwise, it is not included. Default is True.</param>
        /// <param name="typeCode">The type of the output array. If dtype is not given, infer the data type from the other input arguments.</param>
        /// <remarks>https://docs.scipy.org/doc/numpy-1.15.0/reference/generated/numpy.linspace.html</remarks>
        public static NDArray linspace(float start, float stop, int num, bool endpoint = true, NPTypeCode typeCode = NPTypeCode.Single)
        {
            if (typeCode == NPTypeCode.Empty)
                throw new ArgumentException("Invalid typeCode", nameof(typeCode));

            NDArray ret = new NDArray(typeCode, new Shape(num), false);
            float step = Converts.ToSingle((stop - start) / (endpoint ? num - 1.0 : num));

            switch (ret.GetTypeCode)
            {
#if _REGEN
	            case NPTypeCode.Boolean:
	            {
                    unsafe
                    {
                        var addr = (bool*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = (start + i * step) != 0;
                    }

                    return ret;
	            }
	            %foreach except(supported_dtypes, "Boolean"),except(supported_dtypes_lowercase, "bool")%
	            case NPTypeCode.#1:
	            {
                    unsafe
                    {
                        var addr = (#2*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.To#1(start + i * step);
                    }

                    return ret;
	            }
	            %
	            default:
		            throw new NotSupportedException();
#else
                case NPTypeCode.Boolean:
                {
                    unsafe
                    {
                        var addr = (bool*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = start + i * step != 0;
                    }

                    return ret;
                }

                case NPTypeCode.Byte:
                {
                    unsafe
                    {
                        var addr = (byte*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToByte(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Int16:
                {
                    unsafe
                    {
                        var addr = (short*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToInt16(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.UInt16:
                {
                    unsafe
                    {
                        var addr = (ushort*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToUInt16(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Int32:
                {
                    unsafe
                    {
                        var addr = (int*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToInt32(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.UInt32:
                {
                    unsafe
                    {
                        var addr = (uint*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToUInt32(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Int64:
                {
                    unsafe
                    {
                        var addr = (long*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToInt64(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.UInt64:
                {
                    unsafe
                    {
                        var addr = (ulong*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToUInt64(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Char:
                {
                    unsafe
                    {
                        var addr = (char*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToChar(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Double:
                {
                    unsafe
                    {
                        var addr = (double*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToDouble(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Single:
                {
                    unsafe
                    {
                        var addr = (float*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToSingle(start + i * step);
                    }

                    return ret;
                }

                case NPTypeCode.Decimal:
                {
                    unsafe
                    {
                        var addr = (decimal*)ret.Address;
                        for (int i = 0; i < num; i++)
                            *(addr + i) = Converts.ToDecimal(start + i * step);
                    }

                    return ret;
                }

                default:
                    throw new NotSupportedException();
#endif
            }
        }
    }
}
