using BehaveAsSakura.Events;
using System;
using BehaveAsSakura.Variables;
using System.Collections.Generic;

namespace BehaveAsSakura.Tests
{
	class BehaviorTreeOwner : IBehaviorTreeOwner
	{
		private uint time;
		private EventBus eventBus = new EventBus();

		public void Tick(uint deltaTime)
		{
			time += deltaTime;
		}

		public override string ToString()
		{
			return "Owner";
		}

		void ILogger.LogDebug(string msg, params object[] args)
		{
			Log( "DEBUG", msg, args );
		}

		void ILogger.LogError(string msg, params object[] args)
		{
			Log( "ERROR", msg, args );
		}

		void ILogger.LogInfo(string msg, params object[] args)
		{
			Log( "INFO", msg, args );
		}

		void ILogger.LogWarning(string msg, params object[] args)
		{
			Log( "WARN", msg, args );
		}

		void Log(string level, string msg, params object[] args)
		{
			Console.WriteLine( "{0}|{1}", level, string.Format( msg, args ) );
		}

		object IVariableContainer.GetValue(string key)
		{
			switch( key )
			{
				case "uint.3000":
					return 3000u;

				default:
					throw new KeyNotFoundException( key );
			}
		}

		uint IBehaviorTreeOwner.CurrentTime
		{
			get { return time; }
		}

		EventBus IBehaviorTreeOwner.EventBus
		{
			get { return eventBus; }
		}
	}
}
