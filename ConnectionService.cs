using System;
using Zorro.Core;

// Token: 0x0200005B RID: 91
public class ConnectionService : GameService<ConnectionService>
{
	// Token: 0x060003C3 RID: 963 RVA: 0x000169E4 File Offset: 0x00014BE4
	public ConnectionService()
	{
		this.StateMachine = new ConnectionService.ConnectionServiceStateMachine();
		this.StateMachine.RegisterState(new DefaultConnectionState());
		this.StateMachine.RegisterState(new JoinSpecificRoomState());
		this.StateMachine.RegisterState(new InRoomState());
		this.StateMachine.RegisterState(new HostState());
		this.StateMachine.SwitchState<DefaultConnectionState>(false);
	}

	// Token: 0x04000438 RID: 1080
	public ConnectionService.ConnectionServiceStateMachine StateMachine;

	// Token: 0x020002FD RID: 765
	public class ConnectionServiceStateMachine : StateMachine<ConnectionState>
	{
	}
}
