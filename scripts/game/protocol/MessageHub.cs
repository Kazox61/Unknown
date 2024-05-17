using System;
using System.Collections.Generic;

namespace Unknown.Game.Protocol; 

public class MessageHub {
	private readonly Dictionary<MessageId, Action<ProtocolMessage>> _messageEvents = new();

	public void AddCallback(MessageId messageId, Action<ProtocolMessage> messageEvent) {
		_messageEvents.TryAdd(messageId, null);
		_messageEvents[messageId] += messageEvent;
	}

	public void RemoveCallback(MessageId messageId, Action<ProtocolMessage> messageEvent) {
		if (_messageEvents.ContainsKey(messageId)) {
			_messageEvents[messageId] -= messageEvent;
		}
	}

	public void RemoveAllCallbacks() {
		_messageEvents.Clear();
	}

	public void InvokeCallback(MessageId messageId, ProtocolMessage serverMessage) {
		if (_messageEvents.TryGetValue(messageId, out var events)) {
			events?.Invoke(serverMessage);
		}
	}
}