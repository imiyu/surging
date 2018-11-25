﻿using Microsoft.Extensions.Logging;
using Surging.Core.Protocol.Mqtt.Internal.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Surging.Core.Protocol.Mqtt.Internal.Services.Implementation
{
   public class WillService: IWillService
    {
        private static ConcurrentDictionary<String, MqttWillMessage> willMeaasges = new ConcurrentDictionary<String, MqttWillMessage>();
        private readonly ILogger _logger;
        private readonly IChannelService _channelService;

        public WillService(ILogger logger, IChannelService channelService)
        {
            _logger = logger;
            _channelService = channelService;
        }

        public void Add(string deviceid, MqttWillMessage willMessage)
        {
            willMeaasges.AddOrUpdate(deviceid, willMessage,(id,message)=>willMessage); 
        }
        
        public void SendWillMessage(string deviceId)
        {
            if (!string.IsNullOrEmpty(deviceId))
            {
                willMeaasges.TryGetValue(deviceId, out MqttWillMessage willMessage);
                if (willMeaasges != null)
                {
                    _channelService.SendWillMsg(willMessage);
                    if (!willMessage.WillRetain)
                    {
                        Remove(deviceId);
                        if (_logger.IsEnabled(LogLevel.Information))
                            _logger.LogInformation($"deviceId:{deviceId} 的遗嘱消息[" + willMessage.WillMessage + "] 已经被删除");

                    }
                }
            }
        }
        
        public void Remove(string deviceid) {
            willMeaasges.TryRemove(deviceid,out MqttWillMessage message);
        }
    }
}
