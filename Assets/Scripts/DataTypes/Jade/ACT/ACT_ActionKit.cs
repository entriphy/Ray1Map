﻿using BinarySerializer;

namespace R1Engine.Jade 
{
	public class ACT_ActionKit : Jade_File 
    {
        public ActionRef[] Actions { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            Actions = s.SerializeObjectArray(Actions, FileSize / (Loader.IsBinaryData ? 4 : 8), name: nameof(Actions));
        }

        public class ActionRef : BinarySerializable {
            public Jade_Reference<ACT_Action> Action { get; set; }
            public uint ActionPointer { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
			    LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                Action = s.SerializeObject<Jade_Reference<ACT_Action>>(Action, name: nameof(Action));
                if (!Loader.IsBinaryData) ActionPointer = s.Serialize<uint>(ActionPointer, name: nameof(ActionPointer));
                
                if (Action.Key.Key != 1)
                    Action?.Resolve();
            }
        }
    }
}