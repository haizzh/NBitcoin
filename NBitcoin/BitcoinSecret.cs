﻿using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin
{
	public class BitcoinSecret : Base58Data
	{

		public BitcoinSecret(string str)
		{
			this.SetString(str);
		}

		public Key Key
		{
			get
			{
				Key ret = new Key(vchData, 32, vchData.Length > 32 && vchData[32] == 1);
				return ret;
			}
		}

		protected override bool IsValid
		{
			get
			{
				bool compressed = IsCompressed;
				if(compressed)
				{
					if(vchData[32] != 1 || (wifData[0] != 'K' && wifData[0] != 'L'))
						return false;
				}
				else
				{
					if(vchData.Length != 32 || wifData[0] != '5')
						return false;
				}

				bool fCorrectVersion = vchVersion[0] == 128;
				return fCorrectVersion;
			}
		}


		public BitcoinSecret Copy(bool? compressed)
		{
			if(compressed == null)
				compressed = IsCompressed;

			if(compressed.Value && IsCompressed)
			{
				return new BitcoinSecret(wifData);
			}
			else
			{
				byte[] result = Encoders.Base58Check.DecodeData(wifData);
				var resultList = result.ToList();

				if(compressed.Value)
				{
					resultList.Insert(resultList.Count, 0x1);
				}
				else
				{
					resultList.RemoveAt(resultList.Count - 1);
				}
				return new BitcoinSecret(Encoders.Base58Check.EncodeData(resultList.ToArray()));
			}
		}

		public bool IsCompressed
		{
			get
			{
				return vchData.Length == 33;
			}
		}

		public override byte[] ExpectedVersion
		{
			get
			{
				return new byte[] { 128 };
			}
		}
	}
}
