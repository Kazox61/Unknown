using System;
using System.Collections.Generic;
using System.Net;
using Unknown.Utilities;

namespace Unknown.Game.Protocol; 

public static class MessageFactory {
	private static readonly Dictionary<MessageId, Type> Messages = new() {
		{ MessageId.MatchJoin, typeof(MatchJoinMessage) },
		{ MessageId.MatchJoinOk, typeof(MatchJoinOkMessage) },
		{ MessageId.PlayerInput, typeof(PlayerInputMessage) },
		{ MessageId.VisionUpdate, typeof(VisionUpdateMessage) },
	};

	public static ProtocolMessage GetMessage(MessageId messageId, IPEndPoint ipEndPoint = null, Reader reader = null) {
		if (Activator.CreateInstance(Messages[messageId], ipEndPoint, reader) is ProtocolMessage message) {
			return message;
		}

		return null;
	}
}