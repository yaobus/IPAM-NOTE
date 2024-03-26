using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPAM_NOTE
{
	public static class IPAddressCalculations
	{

		public static long AddressCount(int maskLength)
		{
			return (long)Math.Pow(2, 32 - maskLength);
		}

		public static IPAddress SubnetMaskFromPrefixLength(int prefixLength)
		{
			uint subnet = 0xffffffff;
			subnet <<= (32 - prefixLength);
			byte[] bytes = BitConverter.GetBytes(subnet);
			Array.Reverse(bytes);
			return new IPAddress(bytes);
		}

		public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
		{
			byte[] ipBytes = address.GetAddressBytes();
			byte[] maskBytes = subnetMask.GetAddressBytes();

			byte[] result = new byte[ipBytes.Length];
			for (int i = 0; i < ipBytes.Length; i++)
			{
				result[i] = (byte)(ipBytes[i] & maskBytes[i]);
			}

			return new IPAddress(result);
		}

		public static IPAddress GetFirstUsable(this IPAddress networkAddress, System.Net.Sockets.AddressFamily addressFamily)
		{
			byte[] bytes = networkAddress.GetAddressBytes();
			bytes[bytes.Length - 1] += 1; // Increment last byte
			return new IPAddress(bytes);
		}

		public static IPAddress GetLastUsable(this IPAddress networkAddress, System.Net.Sockets.AddressFamily addressFamily, int maskLength)
		{
			byte[] bytes = networkAddress.GetAddressBytes();
			int usableAddresses = (int)Math.Pow(2, 32 - maskLength) - 2; // Calculate the number of usable addresses
			int lastByteIndex = bytes.Length - 1;
			int carry = usableAddresses / 256;
			bytes[lastByteIndex] += (byte)(usableAddresses % 256); // Add remainder to last byte
			for (int i = lastByteIndex - 1; i >= 0 && carry > 0; i--)
			{
				int sum = bytes[i] + carry;
				bytes[i] = (byte)(sum % 256);
				carry = sum / 256;
			}
			return new IPAddress(bytes);
		}


		public static IPAddress GetBroadcastAddress(this IPAddress networkAddress, int maskLength)
		{
			byte[] bytes = networkAddress.GetAddressBytes();
			int lastByteIndex = bytes.Length - 1;
			int subnetBits = 32 - maskLength;

			for (int i = 0; i < subnetBits; i++)
			{
				int byteIndex = i / 8;
				int bitOffset = i % 8;
				bytes[lastByteIndex - byteIndex] |= (byte)(1 << bitOffset);
			}

			return new IPAddress(bytes);
		}


		public static int CalculateSubnetMaskLength(IPAddress subnetMask)
		{
			byte[] bytes = subnetMask.GetAddressBytes();
			uint mask = BitConverter.ToUInt32(bytes.Reverse().ToArray(), 0);
			int maskLength = 0;
			while (mask != 0)
			{
				maskLength++;
				mask <<= 1;
			}
			return maskLength;
		}


	}
}
