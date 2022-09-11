/* @refresh reload */
import { hashIntegration, Router } from '@solidjs/router'
import { render } from 'solid-js/web'

// import './index.css';
import App from './App'
import { RimionshipContextProvider } from './RimionshipContext'
import { SignalRHandler } from './SignalRHandler'

render(() =>
  <RimionshipContextProvider>
    <SignalRHandler />
    <Router source={hashIntegration()}>
      <App />
    </Router>
  </RimionshipContextProvider>, document.getElementById('root') as HTMLElement)
