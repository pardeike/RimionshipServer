/* @refresh reload */
import { render } from 'solid-js/web';

// import './index.css';
import App from './App';
import { RimionshipContextProvider } from './SignalR';
import { SignalRHandler } from './SignalRHandler';

render(() => 
<RimionshipContextProvider>
    <SignalRHandler />
    <App />
</RimionshipContextProvider>, document.getElementById('root') as HTMLElement);
