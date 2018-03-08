using System;
using System.IO;
using Sirona.Utilities.Compression.Engine.Zip.Compression;

namespace Sirona.Utilities.Compression{

	public class EZStream
	{
	
		private const int BUFFERSIZE  = 8192;

		public static void Compress(Stream stIn, Stream stOut)
		{
			
			Deflater def = new Deflater();
			int iLastReadCount = 0;
			byte[] bufRead = new byte[BUFFERSIZE];
			byte[] bufWrite = new byte[BUFFERSIZE];
						
			if (stIn != null && stOut != null)
			{
				for (;;)
				{
				
					iLastReadCount = stIn.Read (bufRead, 0, BUFFERSIZE);
					if (iLastReadCount == 0)
					{
						def.Finish();
						while (!def.IsFinished)
						{
							iLastReadCount = def.Deflate(bufWrite, 0, BUFFERSIZE);
							if (iLastReadCount == 0)
								break;
					
							stOut.Write(bufWrite, 0, iLastReadCount);
						}
						break;
					}
					else
					{
						def.SetInput(bufRead, 0, iLastReadCount);
						while (!def.IsNeedingInput)
						{
							iLastReadCount = def.Deflate(bufWrite, 0, BUFFERSIZE);
							stOut.Write(bufWrite, 0, iLastReadCount);
						}
					}
				}//end for
			}//end if
		}//end function
		
		public static void UnCompress(Stream stIn, Stream stOut)
		{
			Inflater inf = new Inflater();
			int iLastReadCount = 0;
			int iLastReadStInCount = 0;
			byte[] bufRead = new byte[BUFFERSIZE];
			byte[] bufWrite = new byte[BUFFERSIZE];
		
			if (stIn != null && stOut != null)
			{
				do
				{
					iLastReadStInCount = stIn.Read (bufRead, 0, BUFFERSIZE);
					inf.SetInput(bufRead, 0, iLastReadStInCount);
					for (;;)
					{
						iLastReadCount = inf.Inflate(bufWrite, 0, BUFFERSIZE);
						if ( iLastReadCount != 0)
							stOut.Write(bufWrite, 0, iLastReadCount);
						
						if (inf.IsFinished)
							break;
						else if (inf.IsNeedingInput)
							break;
			
					}
				} while (iLastReadStInCount != 0);
			}//end if
		}//end function
	}
}