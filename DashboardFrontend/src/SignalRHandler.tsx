import { batch, createEffect, onCleanup, onMount, VoidComponent } from "solid-js";
import * as signalR from '@microsoft/signalr';
import * as signalRm from '@microsoft/signalr-protocol-msgpack';
import { useRimionship } from "./SignalR";

export const CreateSignalRConnection = () => {
  return new signalR.HubConnectionBuilder()
  .withUrl("api/dashboard")
  .withHubProtocol(new signalRm.MessagePackHubProtocol())
  .withAutomaticReconnect({ nextRetryDelayInMilliseconds: (ctx) => Math.min(ctx.previousRetryCount, 5) * 1000 })
  .build();
};

export const SignalRHandler: VoidComponent = () => {
  const { 
    connection,
    setLatestStats,
    connected, setConnected,
    disconnectReason, setDisconnectReason 
  } = useRimionship();

  let updateTimer: ReturnType<typeof setInterval>;

  const POLLING_INTERVAL = 5000;

  const updateData = async () => {
    try {
      if (connected()) {
        const data = await connection.invoke('GetLatestStats');
        setLatestStats(data);
      }
    }
    catch (err) {
      console.error('Could not fetch data: ', err);
      // TODO: display this in a more generic error way somewhere, I guess?
    }
    finally {
      setTimeout(updateData, POLLING_INTERVAL);
    }
  };

  onMount(async () => {
    while (true) {
      try {
        await connection.start();
        batch(() => {
          setConnected(true);
          setDisconnectReason(undefined);
        });
        break;
      }
      catch (err) {
        setDisconnectReason(err as Error);
        console.error('could not connect to SignalR: ', err);
        await new Promise(f => setTimeout(f, 1000));
      }
    }

    await updateData();
  });

  onCleanup(() => {
    connection.stop();
    clearTimeout(updateTimer);
  });



  createEffect(() => {
    const err = disconnectReason();
    if (!!err && err.message.match(/Status code '40[13]'/)) {
      window.location.href = '/Login?returnUrl=/dashboard.html';
    }
  });

  connection.onclose((err) => batch(() => {
    setConnected(false);
    setDisconnectReason(err);
  }));

  connection.onreconnecting((err) => batch(() => {
    setConnected(false);
    setDisconnectReason(err);
  }));

  connection.onreconnected(() => batch(() => {
    setConnected(true);
    setDisconnectReason(undefined);
  }));

  return <></>;
};