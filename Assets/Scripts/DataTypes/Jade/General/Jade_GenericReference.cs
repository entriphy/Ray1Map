﻿using R1Engine.Serialize;
using System;

namespace R1Engine.Jade {
	public class Jade_GenericReference : R1Serializable {
		public Jade_Key Key { get; set; }
		public Jade_FileType FileType { get; set; }
		public Jade_File Value { get; set; }
		public Jade_FileType.FileType Type => FileType.Type;
		public bool IsNull => Type == Jade_FileType.FileType.None || Key.IsNull;

		public override void SerializeImpl(SerializerObject s) {
			Key = s.SerializeObject<Jade_Key>(Key, name: nameof(Key));
			FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
		}

		public Jade_GenericReference() { }
		public Jade_GenericReference(Context c, Jade_Key key, Jade_FileType fileType) {
			Context = c;
			Key = key;
			FileType = fileType;
		}

		public void Resolve(Action<SerializerObject, Jade_File> onPreSerialize = null, Action<SerializerObject, Jade_File> onPostSerialize = null) {
			if(IsNull) return;
			LOA_Loader loader = Context.GetStoredObject<LOA_Loader>("loader");
			loader.RequestFile(Key, (s, configureAction) => {
				switch(Type) {
					case Jade_FileType.FileType.AI_Instance:
						Value = s.SerializeObject<AI_Instance>((AI_Instance)Value, onPreSerialize: f => {
							configureAction(f); onPreSerialize?.Invoke(s, f);
						}, name: nameof(Value));
						break;
					case Jade_FileType.FileType.AI_Model:
						Value = s.SerializeObject<AI_Model>((AI_Model)Value, onPreSerialize: f => {
							configureAction(f); onPreSerialize?.Invoke(s, f);
						}, name: nameof(Value));
						break;
					case Jade_FileType.FileType.AI_Vars:
						Value = s.SerializeObject<AI_Vars>((AI_Vars)Value, onPreSerialize: f => {
							configureAction(f); onPreSerialize?.Invoke(s, f);
						}, name: nameof(Value));
						break;
					case Jade_FileType.FileType.AI_Function:
						Value = s.SerializeObject<AI_Function>((AI_Function)Value, onPreSerialize: f => {
							configureAction(f); onPreSerialize?.Invoke(s, f);
						}, name: nameof(Value));
						break;
				}
				onPostSerialize?.Invoke(s, Value);
			}, (f) => {
				Value = f;
			});
		}
	}
}
