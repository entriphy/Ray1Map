﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.Jade {
	public class AI_Links_BGE_PCDemo : AI_Links_BGE_PS2_20030814 {
		protected override void InitFunctionDefs() {
			base.InitFunctionDefs();

			#region Function Defs (Unnamed)
			uint[] overrides = new uint[] {
				0x3500B96A,
				0x3500B96B,
				0x3500BAC4,
				0x3500BAC6,
				0x3D003DF7,
				0x3D003E04,
				0x3D003E06,
				0x3D003892,
				0x5E00632D,
				0x5E006324,
				0x3D003882,
				0x3D003A94,
				0x49027635,
				0x490289B1,
				0x490294BF,
				0x5E0084E3,
				0x3D003D54,
				0x3D0030EB,
				0x3D0030F2,
				0x3D0030F4,
				0x3D003D96,
				0x3D003D98,
				0x3D003D9A,
				0x3D001171,
				0x3D001175,
				0x3D00117A,
				0x3D002B35,
				0x3D000710,
				0x3D000711,
				0x3D000625,
				0x3D000629,
				0x3D000630,
				0x3D00063F,
				0x72004DD5,
				0x72004DD6,
				0x72004DDE,
				0x720053D8,
				0x720053D3,
				0x720053D4,
				0x72004DE4,
				0x7200533A,
				0x72005347,
				0x72005349,
				0x72005407,
				0x72002CF6,
				0x72002CF7,
				0x490273BC,
				0x26000155,
				0x26000156,
				0x26000225,
				0x26000158,
				0x26000159,
				0x2600015A,
				0x2600015B,
				0x26000163,
				0x2600028A,
				0x2600028D,
				0x260005D3,
				0x7100F76C,
				0x7100F76D,
				0x7100F76E,
				0x7100F773,
				0x7100F774,
				0x7100F791,
				0x7100F794,
				0x7100F795,
				0x7100F7B9,
				0x7100F7BB,
				0x7100F7C0,
				0x7100F7C1,
				0x7100F7C2,
				0x7100F7D3,
				0x26000049,
				0x2600004A,
				0x2600004D,
				0x26000056,
				0x26000058,
				0x26000353,
				0x2600048B,
				0x2600058F,
				0x26000590,
				0x260005F0,
				0x72000A2A,
				0x72000A2B,
				0x72000A2C,
				0x72000A44,
				0x26000D31,
				0x26000C9B,
				0x26000D32,
				0x260013F8,
				0x3D003032,
				0x3D003139,
				0x5E007D1D,
				0x5E007D21,
				0x5E000485,
				0x5E008A18,
				0x5E005F3D,
				0x5E005F61,
				0x3D000CA7,
				0x3D00099D,
				0x3D000D36,
				0x3D000D2D,
				0x3D000D38,
				0x3D000D3A,
				0x3D000D3E,
				0x3D000D42,
				0x87000D31,
				0x3D0006FA,
				0x3D000701,
				0x3D000703,
				0x5B0004DC,
				0x5B0004DF,
				0x5B0004E1,
				0x5B0004F7,
				0x3D00065D,
				0x3500DF57,
				0x3500DF58,
				0x3500DF59,
				0x3500DF5E,
				0x3500DF5F,
				0x72004F27,
				0x72004F28,
				0x3D0016FE,
				0x7200472C,
				0x7200472D,
				0x72000C2A,
				0x3D003B7E,
				0x9E005BD8,
				0x9E005BD9,
				0x9E006103,
				0x9E006105,
				0x720040A9,
				0x720040AA,
				0x720055BA,
				0x720055BB,
				0x72005F2D,
				0x72005F2E,
				0x72005774,
				0x72005775,
				0x72005776,
				0x72005557,
				0x72005558,
				0x72005559,
				0x7200555A,
				0x7200555B,
				0x7200555C,
				0x72005563,
				0x7200556E,
				0x2600345E,
				0x720054EE,
				0x720054EF,
				0x720054F5,
				0x720054F7,
				0x720054FC,
				0x720054FD,
				0x720055F7,
				0x2600345C,
				0x7200568D,
				0x7200568E,
				0x3D002F6D,
				0xBF002517,
				0xBF002518,
				0x4902121B,
				0x4902121C,
				0x4902122E,
				0x72005044,
				0x7200543B,
				0x72005046,
				0x72005047,
				0x2600345A,
				0x72005045,
				0x3D002E26,
				0x3D002E2D,
				0x3D002E2E,
				0x3D002E30,
				0x3D002DFC,
				0x3D002DFD,
				0x3D002DFE,
				0x3D002E06,
				0x3D002D60,
				0x3D002D62,
				0x3D002D64,
				0x3D002D66,
				0x3D002D72,
				0x3D002D73,
				0x3D002D74,
				0x3D002D76,
				0x3D002D82,
				0x3D002D83,
				0x3D002E3D,
				0x3D00309D,
				0x3D002D22,
				0x3D002D34,
				0x3D002D35,
				0x3D002D37,
				0x3D0032BF,
				0x32000198,
				0x32000199,
				0x320001A0,
				0x320001A1,
				0x320001A3,
				0x3D002CD9,
				0x3D002F5F,
				0x3D002F62,
				0x3D002CC9,
				0x3D002CCA,
				0x3D002987,
				0x3D00298B,
				0x3D002991,
				0x3D002999,
				0x720046D6,
				0x720046D9,
				0x3500BCCC,
				0x3500BCCD,
				0x3500BCF3,
				0x260033FD,
				0x2600244A,
				0x2600244B,
				0x2600244F,
				0x26002457,
				0x2600217B,
				0x2600217C,
				0x2600217D,
				0x26002181,
				0x26002183,
				0x26002EE2,
				0x26001FB9,
				0x26001FC1,
				0x26001FC2,
				0x26001FC3,
				0x32000016,
				0x32000018,
				0x9E0063FD,
				0x72004FC9,
				0x72003FEB,
				0x72003FF1,
				0x72004209,
				0x7200420A,
				0x72003A0D,
				0x72003A10,
				0x3D000587,
				0x3D00058D,
				0x3D00058F,
				0x3D0009E5,
				0x490198B4,
				0x72001361,
				0x72001362,
				0x72001363,
				0x3500B276,
				0x3500B278,
				0x72001350,
				0x72001351,
				0x72001352,
				0x720013AB,
				0x72000BBE,
				0x72000BBF,
				0x72000BC0,
				0x72001516,
				0x3500A3A7,
				0x720007FF,
				0x72000800,
				0x9E0031DD,
				0x9E0031DE,
				0x7100F3AC,
				0x7100F3AD,
				0x7100F3AE,
				0x7100F3AF,
				0x7100F3B0,
				0x7100F3B1,
				0x7100F3B2,
				0x7100F3B3,
				0x7100F3B4,
				0x7100F3BA,
				0x7100F3BB,
				0x7100F3BC,
				0x7100F3BD,
				0x7100F3BE,
				0x3500A887,
				0x3500A888,
				0x3500A889,
				0x3500A897,
				0x72002FB3,
				0x7200377B,
				0x7200377D,
				0x72003787,
				0x7200379F,
				0x720037B1,
				0x720037F7,
				0x720037F9,
				0x9E004083,
				0x72003A2B,
				0x72003B72,
				0x720045A8,
				0x72004ADF,
				0x3D002E33,
				0x7200542E,
				0x720055ED,
				0x3500E90F,
				0x720005D3,
				0x720005D4,
				0x72000AF4,
				0x72000AFC,
				0x72000CB9,
				0x4900FE08,
				0x4900FE09,
				0x35009407,
				0x35009408,
				0x35009409,
				0x49001D80,
				0x49001D82,
				0x7100BB9E,
				0x7100BB9F,
				0x62001247,
				0x3D0013AB,
				0x49021F24,
				0x49021F2C,
				0x49022ED4,
				0x49021E4D,
				0x49021E50,
				0x49021E55,
				0x49023208,
				0x26003462,
				0x26003469,
				0x7100E7A6,
				0x7100E7A7,
				0x7100E7B1,
				0x350091C2,
				0x350091C8,
				0x350091C9,
				0x350091CA,
				0x350095FA,
				0x3500B021,
				0x3500B724,
				0x3500B726,
				0x3500B728,
				0x3500B84D,
				0x26004C6C,
				0x350091AD,
				0x350091AE,
				0x3500A52F,
				0x35007412,
				0x35007498,
				0x7100E7B7,
				0x7100E7B5,
				0x350082FF,
				0x7100E7B3,
				0x3500C01F,
				0x3500C020,
				0x3500C021,
				0x3500C022,
				0x3500BE46,
				0x3500BE47,
				0x3500BE0C,
				0x3500BE11,
				0x3500BE12,
				0x3500BE30,
				0x3500BAE4,
				0x3500BAE5,
				0x3500B8FE,
				0x3500B902,
				0x3500B7BF,
				0x3500B7C3,
				0x3500B85B,
				0x3500B964,
				0x3500B965,
				0x3500BAF6,
				0x3500BAF7,
				0x3500BAFB,
				0x3500B773,
				0x3500B774,
				0x3500B77E,
				0x3500ADB9,
				0x3500ADBA,
				0x3500B906,
				0x3500A952,
				0x3500A956,
				0x3500ADE3,
				0xBF000291,
				0x72005508,
				0x72005509,
				0x72005525,
				0x72005528,
				0x72005529,
				0x72005530,
				0x72005571,
				0x7200554E,
				0x72005572,
				0x72005679,
				0x720056C8,
				0x9E00668A,
				0x9E006690,
				0x9E006691,
				0x9E006692,
				0x9E006695,
				0x9E006697,
				0x9E006699,
				0x9E00669B,
				0x9E00672B,
				0x9E00672C,
				0x26003614,
				0x26003615,
				0x26003638,
				0x49020BEC,
				0x49020BED,
				0x49020BEE,
				0x49020BF8,
				0x49020E9B,
				0x72004C50,
				0x72004C51,
				0x7200522C,
				0x72005213,
				0x72004C64,
				0x720055D3,
				0x72004C66,
				0x720051A6,
				0x720051D4,
				0x72005275,
				0x72004D6B,
				0x72004D6C,
				0x72005236,
				0x72004D70,
				0x7200528C,
				0x7200529B,
				0x72004C57,
				0x7200554A,
				0x720056A4,
				0x72006289,
				0x7200628B,
				0x7200628F,
				0x72006290,
				0x260028F9,
				0x260028FA,
				0x26002913,
				0x2600288C,
				0x26002893,
				0x26002894,
				0x260028DF,
				0x260028E0,
				0x260028EF,
				0x260028F2,
				0x2600287E,
				0x2600287F,
				0x26002880,
				0x260028C0,
				0x260028C1,
				0x3D0028F1,
				0x3D0028F9,
				0x3D002916,
				0x3D002918,
				0x3D002926,
				0x3D002936,
				0x3D002938,
				0x3D0030E0,
				0x2600319F,
				0x3D000CD0,
				0x3D000CD6,
				0x3D000CD8,
				0x3D000CDB,
				0x3D000CDD,
				0x3D000CDF,
				0x3D000CE1,
				0x3D000CE4,
				0x3D000CE6,
				0x3D000CE8,
				0x3D000CEA,
				0x3D000CEF,
				0x3D001D8B,
				0x720026AC,
				0x720026AD,
				0x720026AE,
				0x720026AF,
				0x720026B0,
				0x720026B1,
				0x720026B2,
				0x720026B3,
				0x720026B4,
				0x720026B5,
				0x720026B6,
				0x720026BC,
				0x720026BF,
				0x720026C3,
				0x72002EB2,
				0x7200270A,
				0x72002B40,
				0x720041C7,
				0x7200482C,
				0x72004CBC,
				0x72004F09,
				0x72004F0F,
				0x72005609,
				0x72005614,
				0x72005615,
				0x7200205D,
				0x7200205E,
				0x7200205F,
				0x72002060,
				0x72002061,
				0x72002067,
				0x72002069,
				0x72002078,
				0x7200207A,
				0x720020A8,
				0x72001DDA,
				0x72001DDB,
				0x72001DDC,
				0x72001DDD,
				0x72001E0A,
				0x72005FA6,
				0x72005FA7,
				0x72005FAE,
				0x72005FAF,
				0x49020711,
				0x49020712,
				0x72004FC3,
				0x7200502C,
				0x7200502F,
				0x72005273,
				0x72004D7A,
				0x72004D7B,
				0x72004D7C,
				0x72004D8C,
				0x72004D90,
				0x26002A29,
				0x26002A2A,
				0x26002A39,
				0x26002A42,
				0x26002A44,
				0x72004C14,
				0x72004C16,
				0x72004C18,
				0x72004C19,
				0x3D001246,
				0x3D001250,
				0x3D001251,
				0x3D00126B,
				0x3D00126E,
				0x3D001151,
				0x3D00115F,
				0x3D001159,
				0x3D00115D,
				0x72001315,
				0x72001316,
				0x72001317,
				0x72001318,
				0x72001319,
				0x7200131A,
				0x72000B1C,
				0x72000B1D,
				0x72000B1E,
				0x72000B1F,
				0x72000B28,
				0x72000B2A,
				0x720008B9,
				0x720008BA,
				0x720008BB,
				0x26001103,
				0x7200067E,
				0x7200067F,
				0x72000680,
				0x7200069B,
				0x72003A31,
				0x7200071F,
				0x72000722,
				0x72001202,
				0x72001C27,
				0x72003ACA,
				0x72003ACB,
				0x35009FC1,
				0x35009FC5,
				0x35009FB8,
				0x35009FB9,
				0x9E001C08,
				0x9E001C09,
				0x9E001C14,
				0x72000C9E,
				0x260015A6,
				0x260015A8,
				0x720060FA,
				0x9E0076FD,
				0x5B0002AE,
				0x5B0002B1,
				0x87000DE7,
				0x35009A5A,
				0x35009A5B,
				0x35009A5C,
				0x3D0005BE,
				0x3D0005BF,
				0x7100AF33,
				0x7100AF34,
				0x7100328E,
				0x7100328F,
				0x71003290,
				0x04000818,
				0x71003614,
				0x71003615,
				0x71004197,
				0x3D001D4A,
				0x3D001D4D,
				0x3D001D52,
				0x3D001D54,
				0x3D001D5D,
				0x3D002949,
				0x3D0039EE,
				0x72004BD2,
				0x72004BD3,
				0x72004BC5,
				0x72004BC6,
				0x72004BC7,
				0x72004000,
				0x72004001,
				0x72004002,
				0x72004021,
				0x72000F0B,
				0x72000F0C,
				0x72000F0D,
				0x72000F0E,
				0x72000F0F,
				0x72000F19,
				0x720048A4,
				0x72003FE7,
				0x72000E6C,
				0x72000E6D,
				0x72001190,
				0x720012D7,
				0x260036C7,
				0x260036E1,
				0x26003700,
				0x26003709,
				0x72005E11,
				0x72005E12,
				0x72005EA7,
				0x72006309,
				0x9E003B31,
				0x9E003B37,
				0x9E0035B8,
				0x9E0035BD,
				0x9E0035BF,
				0x7100CAF9,
				0x7100CAFA,
				0x7100CAFD,
				0x9E00459B,
				0x9E001C42,
				0x7100BF38,
				0x7100BF39,
				0x7100BF3A,
				0x7100C4E5,
				0x26003D8C,
				0x7100BF01,
				0x7100BF02,
				0x7100BF03,
				0x7100BF07,
				0x7100BF42,
				0x7100BF43,
				0x7100BF46,
				0x7100C57F,
				0x26000748,
				0x260035B1,
				0x260035B5,
				0x7100BD48,
				0x7100BD49,
				0x9E001644,
				0x9E001B3E,
				0x9E001B3C,
				0x9E001B40,
				0x9E002E17,
				0x9E002DFC,
				0x9E001D18,
				0x72000573,
				0x72000576,
				0x720006D9,
				0x9E000651,
				0x9E000652,
				0x9E000653,
				0x72000A9A,
				0x35008E69,
				0x35008E6A,
				0x35008E6B,
				0x35008E6C,
				0x4D000811,
				0x35009242,
				0x35009246,
				0x7100BA8A,
				0x7100BA88,
				0x320000BA,
				0x320000BB,
				0xBD00017B,
				0xBD000181,
				0x9E005F13,
				0x9E005FA7,
				0x9E005EA4,
				0x9E005B47,
				0x9E005B48,
				0x9E005B1F,
				0x9E005B23,
				0x26000D0F,
				0x26003173,
				0x2600023F,
				0x26000240,
				0x26000241,
				0x26000242,
				0x26000232,
				0x26000233,
				0x26000235,
				0x26000DC5,
				0x3D0096A9,
				0x3D005174,
				0x3D005175,
				0x3D003ED2,
				0x72005581,
				0x72005582,
				0x72005583,
				0x72005584,
				0x72005585,
				0x720050EE,
				0x720050EF,
				0x720050F6,
				0x720050F7,
				0x720050FA,
				0x72005122,
				0x72005458,
				0x72005067,
				0x72005069,
				0x7200506F,
				0x7200508A,
				0xBF003B13,
				0x3D0021AF,
				0x3D0021B0,
				0x3D0021B4,
				0x3D0021B6,
				0x3D0021C7,
				0x3D0021BA,
				0x3D0021CE,
				0x72004FBE,
				0x72004C09,
				0x72004A8E,
				0x72004A8F,
				0x3D002CDF,
				0x3D002CE0,
				0x870003EC,
				0x3D00190F,
				0x3D001913,
				0x3D00191A,
				0x72004B07,
				0x3D001BEE,
				0x3D0042D9,
				0x2600382A,
				0x720046AD,
				0x720046AE,
				0x720046AF,
				0x720046C5,
				0x72004AD5,
				0x72004B25,
				0x72004B26,
				0x9E005579,
				0x3D000FF8,
				0x3D000FF9,
				0x3D000FFF,
				0x3D001008,
				0x3D001165,
				0x3D001166,
				0x3D000E45,
				0x3D000E51,
				0x3D000E52,
				0x72004D0F,
				0x72004D96,
				0x72004D98,
				0x3D000D9C,
				0x3D000DA3,
				0x3D000DA5,
				0x3D000DA7,
				0x3D000DB0,
				0x3D000DB2,
				0x3D0016BC,
				0x72006276,
				0x720026E6,
				0x720026E7,
				0x720026E8,
				0x720026E9,
				0x720027DF,
				0x720026ED,
				0x72002701,
				0x72002703,
				0x72004A0E,
				0x72004A10,
				0x72004A15,
				0x720060FF,
				0x7200682C,
				0x720025EB,
				0x720025EC,
				0x720025ED,
				0x72003815,
				0x72003816,
				0x72003819,
				0x7200381C,
				0x72004381,
				0x720043F1,
				0x870010AD,
				0x7200198C,
				0x7200198D,
				0x7200198E,
				0x7200198F,
				0x720019E7,
				0x72001A8E,
				0x72002FE7,
				0x72004736,
				0x72004737,
				0x72004D66,
				0x72004D67,
				0x72004D93,
				0x72004D94,
				0x3D002F59,
				0x3500A7A0,
				0x3500A7A4,
				0x5E00797B,
				0x3500BC90,
				0x3500BC91,
				0x72000B50,
				0x72000B51,
				0x72000B52,
				0x72000B53,
				0x72000B54,
				0x72000B55,
				0x3500A464,
				0x3500A7F4,
				0x3500A7F5,
				0x3500A7F7,
				0x3500ADB3,
				0x9E0035A2,
				0x9E0035A9,
				0x9E0035AB,
				0x26000785,
				0x9E003F5A,
				0x26003089,
				0x2600308D,
				0x2600308F,
				0x26003094,
				0x3D0034BD,
				0x3D0052CC,
				0x3D0052CE,
				0x490289B8,
				0x9E00302C,
				0x9E00302D,
				0x9E00302E,
				0x7100CB21,
				0x7100C543,
				0x9E002FBA,
				0x9E002FC2,
				0x9E003026,
				0x9E00152E,
				0x9E000793,
				0x26002F35,
				0x9E00079A,
				0x9E003F3D,
				0x9E00079C,
				0x9E0065B2,
				0x7200564B,
				0x7200564C,
				0x71005C6B,
				0x71005C75,
				0x72001E9E,
				0x72001E9F,
				0x72001EA0,
				0x72001EA1,
				0x72001EA2,
				0x72001EA3,
				0x72001EA4,
				0x72001EA5,
				0x72001EA6,
				0x72001EBC,
				0x7200437B,
				0x26002206,
				0x72001EBF,
				0x72001EC0,
				0x72001EC2,
				0x72001EC3,
				0x72001EC4,
				0x72001EC5,
				0x72002D55,
				0x72001EC7,
				0x72001EC8,
				0x72002006,
				0x72002007,
				0x72002053,
				0x72002080,
				0x720023B3,
				0x7200277B,
				0x7200277D,
				0x72002B8F,
				0x720043D5,
				0x7200469A,
				0x7200469C,
				0x720046C1,
				0x87000CA6,
				0x72006092,
				0x720060A9,
				0x7100BBCA,
				0x7100BBCB,
				0x7100BBCE,
				0x72000DB5,
				0x72000775,
				0x7100BBE9,
				0x7100BBEA,
				0x7100BBED,
				0x7100BBEE,
				0x72000670,
				0x7100F314,
				0x7100C4EC,
				0x7100C4ED,
				0x7100C4EE,
				0x7100C4F1,
				0x7100C4F2,
				0x7100C4F6,
				0x5E006207,
				0x5E0062CF,
				0x5E0062D0,
				0x72003B81,
				0x7200085B,
				0x7100D954,
				0x7100D956,
				0x7100D958,
				0x72000669,
				0x72000673,
				0x72003C6A,
				0x72003C68,
				0x720007EA,
				0x720007EC,
				0x720007F6,
				0x720008FE,
				0x72000907,
				0x72000DB7,
				0x720019DB,
				0x72001C82,
				0x72001C6F,
				0x72001C70,
				0x72001CAC,
				0x72001CAE,
				0x72003B76,
				0x72003C70,
				0x72003B82,
				0x72003B83,
				0x72003B84,
				0x72003B85,
				0x72003B86,
				0x72003B87,
				0x72003B88,
				0x72003B89,
				0x72003C6E,
				0x72003C6C,
				0x72003C76,
				0x72004379,
				0x7200437D,
				0x72004566,
				0x7200459E,
				0x7200461A,
				0x72004A0A,
				0x72004A27,
				0x72004C05,
				0x72001D84,
				0x72005621,
				0x72004CFE,
				0x72004D0B,
				0x72005574,
				0x72005035,
				0x72005F36,
				0x71000CD9,
				0x26003C23,
				0x71000CDB,
				0x6C004BB6,
				0x26003A64,
				0x260032BD,
				0x2600307F,
				0x2600304A,
				0x26002FBC,
				0x320000F1,
				0x26002A78,
				0x71004F1D,
				0x6C003848,
				0x26002A70,
				0x6C00530A,
				0x6C006129,
				0x7100B98B,
				0x6C006134,
				0x71004BD8,
				0x71004BD9,
				0x7100B5A2,
				0x71005CE1,
				0x260029CC,
				0x260027CC,
				0x26001F9F,
				0x7100AE74,
				0x260011E1,
				0x26000874,
				0x26000787,
				0x7100AECE,
				0x7100AED2,
				0x26000551,
				0x7100AF12,
				0x6C00AB5A,
				0x2600032F,
				0x9E000035,
				0x260011DF,
				0x9E00049A,
				0x9E00049B,
				0x9E00049D,
				0x9B000008,
				0x9E00063F,
				0x26000079,
				0x7100F7EA,
				0x9E0006BA,
				0x7100B5B9,
				0x7100F55C,
				0x7100B715,
				0x7100B717,
				0x7100B724,
				0x7100B725,
				0x7100B726,
				0x7100DF89,
				0x9E00075D,
				0x7100B72F,
				0x7100B73E,
				0x7100B77D,
				0x7100B77F,
				0x7100B7AB,
				0x7100B928,
				0x7100B9D8,
				0x7100CC55,
				0x7100BA28,
				0x7100BA29,
				0x7100F3A2,
				0x7100BA42,
				0x7100BA44,
				0x7100BD0F,
				0x7100BD43,
				0x7100BEE4,
				0x7100F349,
				0x2600032C,
				0x7100BF0A,
				0x7100BF3E,
				0x7100C4DF,
				0x7100C535,
				0x26003C27,
				0x35001FF5,
				0x35001FF6,
				0x71002B2C,
				0x9E003423,
				0x9E003298,
				0x9E003294,
				0x9E003292,
				0x6C00AA9D,
				0x6C00538A,
				0x350051AA,
				0x9E002DE4,
				0x9E002C81,
				0x9E002B97,
				0x9E001568,
				0x9E001569,
				0x9E00156A,
				0x71004B37,
				0x71004B38,
				0x71004B4A,
				0x6C0075DE,
				0x9E00240A,
				0x6C007D45,
				0x9E006A80,
				0x9E005A3D,
				0x9E001F93,
				0x72004204,
				0x6C00AA8F,
				0x9E001ECD,
				0x9E001C94,
				0x9E001C1A,
				0x9E001B3A,
				0x6C00AAA1,
				0x6C00AAA3,
				0x6C00AAAE,
				0x6C00AAB0,
				0x6C00AADB,
				0x9E00387F,
				0x6C00AB1A,
				0x6C00AB27,
				0x9E0054A6,
				0x9E001B2F,
				0x6C00AB4C,
				0x9E00049F,
				0x9E0004A1,
				0x9E0006E0,
				0x9E0037BA,
				0x9E0035FC,
				0x9E0008D1,
				0x9E0008DF,
				0x9E0035E6,
				0x9E000A3C,
				0x9E000A3E,
				0x9E000A40,
				0x9E000A55,
				0x9E005352,
				0x9E00418B,
				0x9E00156B,
				0x9E000A8E,
				0x9E000A90,
				0x9E000A92,
				0x9E000A96,
				0x9E000AE3,
				0x9E0014F9,
				0x9E0015CB,
				0x9E001557,
				0x9E0015B3,
				0x9E0015CC,
				0x9E0015CE,
				0x9E0015D3,
				0x9E001606,
				0x9E001612,
				0x9E001613,
				0x9E001615,
				0x9E001617,
				0x9E001619,
				0x9E001634,
				0x9E00166C,
				0x9E00166E,
				0x9E0040D9,
				0x9E003DAE,
				0x9E003D3C,
				0x9E003C61,
				0x9E001B26,
				0x9E0077CA,
				0x260034BC,
				0x32000036,
				0x32000037,
				0x3200003C,
				0x3200003D,
				0x9E0060C1,
				0x9E0060C2,
				0x9E0060C4,
				0x9E0060C6,
				0x9E0060C8,
				0x9E0060CA,
				0x9E0060CC,
				0x9E0060D7,
				0x9E0060D9,
				0x9E0060DB,
				0x9E006135,
				0x9E00614C,
				0x9E006159,
				0x9E006150,
				0x9E006183,
				0x9E0063C2,
				0x9E0063C3,
				0x9E0063C4,
				0x9E006443,
				0x9E00645C,
				0x9E006465,
				0x9E006468,
				0x9E006555,
				0x9E006629,
				0x9E0066CA,
				0x9E006818,
				0x9E006859,
				0x9E006890,
				0x9E006BD7,
				0x9E006CAB,
				0x3D001AE5,
				0x3D001AE7,
				0x3D002E66,
				0x3D001AF0,
				0x3D001AF9,
				0x5E006E7C,
				0x7100BD29,
				0x7100BD16,
				0x26000608,
				0x2600279D,
				0x5E00021D,
				0x5E00021E,
				0x5B000384,
				0x5B000387,
				0x5E000004,
				0x5E00002C,
				0x5E000040,
				0x7100E3A6,
				0x7100F57A,
				0x71003F6C,
				0x3500A258,
				0x7100BEC6,
				0x7100BF1B,
				0x7100DF8E,
				0x7100E62F,
				0x7100E631,
				0x260002DF,
				0x26000549,
				0x26000606,
				0x260034E3,
				0x71003B28,
				0x71003B29,
				0x71008011,
				0x7100AE5F,
				0x7100AE61,
				0x260032BF,
				0x26001A8D,
				0x260011F9,
				0x2600080E,
				0x2600055D,
				0x49018B8D,
				0x7100DF93,
				0x9E000A32,
				0x7100BA78,
				0x7100BB90,
				0x7100BBE5,
				0x7100BD05,
				0x7100BEF2,
				0x71003FF1,
				0x71004002,
				0x710046D6,
				0x7100C854,
				0x7A000AF7,
				0x7100D95B,
				0x26003350,
				0x260007CA,
				0x710047B9,
				0x26000686,
				0x7100AED6,
				0x7100C4F9,
				0x26000684,
				0x9E00062A,
				0x9E0053F2,
				0x260007CC,
				0x9E005BE9,
				0x9E005BEA,
				0x9E005BEB,
				0x9E00615C,
				0x9E00615F,
				0x9E006160,
				0x26003557,
				0x9E006AEE,
				0x9F000065,
				0x32000062,
				0x9E004478,
				0x9E004479,
				0x9E0040DE,
				0x9E003DCD,
				0x9E003DCF,
				0x9E003DE3,
				0x9E003D8A,
				0x9E003D8B,
				0x9E003B97,
				0x9E003B98,
				0x9E003B99,
				0x9E003B9B,
				0x26000143,
				0x7100F56D,
				0x7100F571,
				0x7100F73B,
				0x7100F73C,
				0x7100F741,
				0x72000AC2,
				0x72000AC9,
				0x9E0035DD,
				0x9E0035DE,
				0x7100E79A,
				0x9E002F71,
				0x9E002F75,
				0x9E002FB1,
				0x7100C16D,
				0x7100C16E,
				0x9E002FB6,
				0x7100E786,
				0x7100E787,
				0x9E002532,
				0x9E002533,
				0x9E002534,
				0x9E002535,
				0x7100F59C,
				0x26003213,
				0x350095C9,
				0x350095CA,
				0x7100E394,
				0x7100E395,
				0x7100E396,
				0x6C005D7B,
				0x9E003D76,
				0x6C004580,
				0x6C004581,
				0x260030A5,
				0x26000014,
				0x35008312,
				0x35008313,
				0x35008314,
				0x35009479,
				0x3500AD36,
				0x7200080A,
				0x7200080B,
				0x7200080C,
				0x7200080D,
				0x3500ADAE,
				0x3D002F8E,
				0x3D002F92,
				0x3D002F97,
				0x3D002F99,
				0x3D002FB6,
				0x3D002FBE,
				0x5E005FA8,
				0x5E005FA9,
				0x3D001CB4,
				0x3D003BE5,
				0x5E0069CE,
				0x35009469,
				0x8D00521A,
				0x9E0007E8,
				0x9E0007ED,
				0x9E0007EF,
				0x9E001B29,
				0x9E0035AD,
				0x7100E3B9,
				0x71001089,
				0x71001D6D,
				0x350008E4,
				0x35009614,
				0x6C001AEA,
				0x5E00846A,
				0x5E008468,
				0x8D00520B,
				0x9E0007A6,
				0x5B000128,
				0x5E0063AE,
				0x5E0069CC,
				0x35006488,
				0x35006622,
				0x8D00520D,
				0x4901FC14,
				0x4901FC1C,
				0x4901FD03,
				0x4901E6E7,
				0x4901E6E8,
				0x4901E6F0,
				0x4901F0E8,
				0x4901F0EC,
				0x4901F0EE,
				0x4901FDD2,
				0x4901F480,
				0x4901BEE3,
				0x4901BEE4,
				0x26000D75,
				0x26000D78,
				0x720020A0,
				0x720020A1,
				0x4901880D,
				0x49018B57,
				0x5E006922,
				0x5E006923,
				0x5E006931,
				0x5E00692F,
				0x5E00699E,
				0x5E0069A6,
				0x49022819,
				0x7100F33C,
				0x49018734,
				0x49018735,
				0x49018A17,
				0x9E003001,
				0x9E003002,
				0x260014D1,
				0x9E003008,
				0x260014D2,
				0x4900FDBB,
				0x4901C2EB,
				0x4900FE1A,
				0x4900FDC0,
				0x4900FDCD,
				0x4900FDD1,
				0x4900FDD3,
				0x4900FDD6,
				0x4900FE1B,
				0x49018E1E,
				0x4900FE5F,
				0x4900FEC6,
				0x4900FDF2,
				0x4900FDF4,
				0x4900FE1F,
				0x4900FE21,
				0x4900FE26,
				0x4901C2EC,
				0x4900FE2A,
				0x4900FE47,
				0x4901C2ED,
				0x4901C2E7,
				0x4900FE54,
				0x4900FE55,
				0x4900FE56,
				0x4900FE57,
				0x4900FE59,
				0x4901C2E5,
				0x4900FFF6,
				0x4900FEBC,
				0x4900FEC0,
				0x4900FEC2,
				0x4900FEC4,
				0x49018B7A,
				0x49018B78,
				0x4900FECE,
				0x49018B76,
				0x49018B67,
				0x4900FF1D,
				0x4901C2DE,
				0x4900FF66,
				0x4901C2DF,
				0x4901C089,
				0x490186A8,
				0x490186B4,
				0x4901C08A,
				0x4901871E,
				0x49018728,
				0x4901874A,
				0x4901874B,
				0x4901874C,
				0x5E0069ED,
				0x4901889E,
				0x490188A7,
				0x490188AA,
				0x490188AB,
				0x49018A1E,
				0x49018B4E,
				0x49018B50,
				0x49018B52,
				0x49018B54,
				0x49018E1F,
				0x49018E20,
				0x49018E22,
				0x49018E29,
				0x49019640,
				0x49019BBA,
				0x49019BBC,
				0x49019BBF,
				0x49019BC2,
				0x4901C08B,
				0x49019BC9,
				0x49019BCB,
				0x49019C6C,
				0x49019F9A,
				0x4901C08C,
				0x4901C084,
				0x4901C07B,
				0x4901BB23,
				0x4901A410,
				0x4901A4EB,
				0x4901A4EC,
				0x49028D88,
				0x49023E6C,
				0x4901AD19,
				0x4901AD1B,
				0x4901B107,
				0x4901B293,
				0x4901B294,
				0x4901B295,
				0x4901B8B4,
				0x4901B97A,
				0x4901BA51,
				0x4901BA56,
				0x4901BA5A,
				0x4901BA5B,
				0x4901BA5D,
				0x4901CCBB,
				0x4901E95B,
				0x49027C9F,
				0x49021D83,
				0x49022E0B,
				0x6C00AD90,
				0x6C00AD93,
				0x9E001C37,
				0x6C00AD9C,
				0x6C00AD9D,
				0x6C00ADA3,
				0x9E000008,
				0x9E00000C,
				0x9E000010,
				0x9E000016,
				0x9E000037,
				0x9E000063,
				0x9E00008A,
				0x9E00062C,
				0x9E0006C3,
				0x9E0006C5,
				0x9E000844,
				0x9E001C34,
				0x9E000847,
				0x9E00084B,
				0x9E00084D,
				0x9E002FF7,
				0x9E000851,
				0x9E000853,
				0x9E000855,
				0x9E000857,
				0x49018899,
				0x9E000A23,
				0x9E000A25,
				0x9E000A26,
				0x9E001C35,
				0x9E000AF5,
				0x9E000AF8,
				0x9E000AFB,
				0x9E003D74,
				0x9E0014FB,
				0x9E0014FD,
				0x9E003C68,
				0x9E0015BD,
				0x9E0015DB,
				0x49018740,
				0x9E001C3E,
				0x4900F65D,
				0x9E0039F9,
				0x4900F663,
				0x9E006DD3,
				0x9E002D62,
				0x49018A1C,
				0x9E0068BA,
				0x4901E2ED,
				0x9E0068B8,
				0x9E0068B6,
				0x9E006771,
				0x9E006773,
				0xFFFFFFFF,
			};
			#endregion

			HashSet<uint> fdLookup = new HashSet<uint>(FunctionDefs.Select(fd => fd.Key));
			List<uint> addedKeys = new List<uint>();
			foreach (var u in overrides) {
				if(fdLookup.Contains(u)) continue;
				addedKeys.Add(u);
			}
			if (addedKeys.Any()) {
				var fdefs = FunctionDefs;
				int len = fdefs.Length;
				Array.Resize(ref fdefs, len + addedKeys.Count);
				for (int i = 0; i < addedKeys.Count; i++) {
					fdefs[len + i] = new AI_FunctionDef(addedKeys[i], $"Custom_{addedKeys[i]:X8}");
				}
				FunctionDefs = fdefs;
			}
		}
	}
}
