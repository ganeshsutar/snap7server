using System;
using Snap7;
using System.Text;

namespace SimpleSnap7Server
{
	class MainClass
	{
		private static S7Server Server;
		private static byte[] DB1 = new byte[1024];
		private static byte[] DB2 = new byte[1024];
		private static byte[] DB3 = new byte[1024];
		private static S7Server.TSrvCallback TheEventCallBack;
		//private static S7Server.TSrvCallback TheReadCallBack;

		static void EventCallBack(IntPtr usrPtr, ref S7Server.USrvEvent uevent, int size)
		{
			Console.WriteLine (Server.EventText (ref uevent));
			Console.WriteLine ("Code={0}, Params={1},{2},{3},{4}, RetCode={5}", 
			                   uevent.EvtCode, uevent.EvtParam1, uevent.EvtParam2, uevent.EvtParam3, uevent.EvtParam4,
			                   uevent.EvtRetCode);
			if (uevent.EvtCode == S7Server.evcDataWrite) {
				PrintRWEvent (ref uevent);
			} else if (uevent.EvtCode == S7Server.evcDataRead) {
				PrintRWEvent (ref uevent);
			}
		}

		static void PrintRWEvent(ref S7Server.USrvEvent uevent)
		{
			StringBuilder builder = new StringBuilder ();

			if (uevent.EvtCode == S7Server.evcDataRead) {
				builder.Append ("[ READ  ] ");
			} else if (uevent.EvtCode == S7Server.evcDataWrite) {
				builder.Append ("[ WRITE ] ");
			} else {
				return;
			}

			switch (uevent.EvtParam1) {
			case S7Server.S7AreaCT:
				builder.Append ("Area: CT, ");
				break;
			case S7Server.S7AreaDB:
				builder.Append ("Area: DB" + uevent.EvtParam2 +", ");
				break;
			case S7Server.S7AreaMK:
				builder.Append ("Area: MK, ");
				break;
			case S7Server.S7AreaPA:
				builder.Append ("Area: PA, ");
				break;
			case S7Server.S7AreaPE:
				builder.Append ("Area: PE, ");
				break;
			case S7Server.S7AreaTM:
				builder.Append ("Area: TM, ");
				break;
			}
			builder.Append (string.Format ("Start: {0}, Size: {1}", uevent.EvtParam3, uevent.EvtParam4));
			Console.WriteLine (builder.ToString ());
		}

		static void ReadCallBack(IntPtr usrPtr, ref S7Server.USrvEvent uevent, int size)
		{
			Console.WriteLine (Server.EventText (ref uevent));
			Console.WriteLine ("Code={0}, Params={1},{2},{3},{4}, RetCode={5}", 
			                   uevent.EvtCode, uevent.EvtParam1, uevent.EvtParam2, uevent.EvtParam3, uevent.EvtParam4,
			                   uevent.EvtRetCode);
		}


		public static void Main (string[] args)
		{
			DB1 [0] = 0xff;
			DB2 [0] = 0xff;
			DB3 [0] = 0xff;
			Server = new S7Server ();
			Server.RegisterArea (S7Server.srvAreaDB, 1, ref DB1, DB1.Length);
			Server.RegisterArea (S7Server.srvAreaDB, 2, ref DB2, DB2.Length);
			Server.RegisterArea (S7Server.srvAreaDB, 3, ref DB3, DB3.Length);

			TheEventCallBack = new S7Server.TSrvCallback (EventCallBack);
			//TheReadCallBack = new S7Server.TSrvCallback (ReadCallBack);

			// Server.EventMask = ~S7Server.evcDataRead;
			Server.SetEventsCallBack (TheEventCallBack, IntPtr.Zero);
			// Server.SetReadEventsCallBack (TheReadCallBack, IntPtr.Zero);

			int Error = Server.Start ();
			if (Error == 0) {
				Console.ReadKey ();
				Server.Stop ();
			} else {
				Console.WriteLine (Server.ErrorText (Error));
			}
		}
	}
}
