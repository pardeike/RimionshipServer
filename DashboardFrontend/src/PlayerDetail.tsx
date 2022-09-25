import { useParams } from "@solidjs/router"
import { JSX, onMount } from "solid-js"
import { createEffect, createMemo, createSignal, For, Show, VoidComponent } from "solid-js"
import { LatestTable } from "./LatestTable"
import { useRimionship } from "./RimionshipContext"
import { CC, displayValue } from "./Utils"
import { UserInfo } from "./MessageDTOs"

const MiniColumns = [
  CC('Place'),
  CC('UserId')
]

const Details = [
  [
    [
      CC('Wealth'),
      CC('Enemies'),
      CC('NumThreatBigs'),
    ], [
      CC('MapCount'),
      CC('MentalColonists'),
      CC('Fire'),
    ], [
      CC('Colonists'),
      CC('DownedColonists'),
      CC('WeaponDps'),
    ], [
      CC('ColonistsNeedTending'),
      CC('Conditions'),
      CC('Electricity'),
    ], [
      CC('MedicalConditions'),
      CC('NumRaidsEnemy'),
      CC('Prisoners'),
    ],
  ], [
    [
      CC('TicksIgnoringBloodGod'),
    ], [
      CC('TicksLowColonistMood'),
    ], [
      CC('ColonistsKilled'),
    ], [
      CC('AmountBloodCleaned'),
    ], [
      CC('AnimalMeatCreated'),
    ],
  ], [
    [
      CC('Rooms'),
      CC('WildAnimals'),
      CC('DamageTakenPawns'),
    ], [
      CC('Medicine'),
      CC('TamedAnimals'),
      CC('DamageTakenThings'),
    ], [
      CC('Food'),
      CC('Visitors'),
      CC('DamageDealt'),
    ], [
      CC('GreatestPopulation'),
      CC('Temperature'),
    ], [
      CC('Caravans'),
      CC('InGameHours'),
    ],
  ],
]

const MiniPlayerList: VoidComponent = () => <LatestTable sortable={false} columns={MiniColumns} width={250} />

const SetDirectionInstruction: VoidComponent<{ userId: string }> = (props) => {
  const { connection, directionList } = useRimionship()

  const [saving, setSaving] = createSignal(false)
  const [disabled, setDisabled] = createSignal(true)
  const [comment, setComment] = createSignal('')

  createEffect((prevUser) => {
    const dir = directionList.find(f => f.UserId === props.userId)?.Comment
    const val = setComment(prev => dir ?? (prevUser === props.userId) ? prev : '')
    inputEle.value = dir ?? ''
    return prevUser
  })

  let inputEle: HTMLInputElement = null!

  const setText = (text: string) => {
    setComment(text)
    setDisabled(false)
  }

  const save = async () => {
    setDisabled(true)
    setSaving(true)
    try {
      await connection.invoke('SetDirectionInstruction', props.userId, comment())
    }
    catch (err) {
      setDisabled(false)
      console.error('Could not save instruction:', err)
    }
    finally {
      setSaving(false)
    }
  }
  return <div class="input-group">
    <input
      type="text"
      class="form-control bigger"
      value={comment()}
      placeholder="Regie-Kommentar"
      onInput={e => setText((e.currentTarget as HTMLInputElement).value)}
      onKeyDown={e => e.key === 'Enter' && save()}
      disabled={saving()}
      ref={inputEle} />
    <button class="btn btn-secondary" type="button" disabled={disabled()} onClick={save}>Speichern</button>
  </div>
}

// ========================================================================================================

export const PlayerDetail: VoidComponent = () => {
  const params = useParams<{ id: string }>()
  const { latestStats, users, switchTwitchChannel } = useRimionship()

  const getStoredStat = () => {
    const name = "stat="
    let decodedCookie = decodeURIComponent(document.cookie)
    let ca = decodedCookie.split(';')
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i]
      while (c.charAt(0) == ' ') {
        c = c.substring(1)
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length)
      }
    }
    return "Wealth"
  }

  const [twitchZoomed, setTwitchZoomed] = createSignal(false)
  const user = createMemo(() => users[params.id])
  const stats = createMemo(() => latestStats.find(s => s.UserId === params.id))
  const [selectedStat, setSelectedStat] = createSignal(getStoredStat())

  var twitchPlayer: any = null

  createEffect((prevUser) => {
    if (!twitchPlayer)
      twitchPlayer = new Twitch.Embed("twitch-embed", {
        width: '100%',
        height: '100%',
        layout: 'video',
        muted: true,
        channel: user().UserName
      })

    const currentUser = user().UserName
    if (currentUser != prevUser) {
      twitchPlayer.setChannel(currentUser)
    }
    return currentUser
  }, user().UserName)

  const statsHeaderColor = (id: string, val: JSX.Element) => {
    if (id == selectedStat())
      return { 'color': 'white', 'background-color': 'black' }
    return { 'color': val == '' ? '#ccc' : '#666' }
  }

  const showGraph = (stat: string) => {
    if (stat == 'Timestamp')
      return () => { }
    else
      return () => {
        setSelectedStat(stat)
        document.cookie = "stat=" + stat
      }
  }

  const twitchZoom = () => {
    const val = !twitchZoomed()
    setTwitchZoomed(val)
  }

  const zoomColumn = () => {
    return {
      'width': twitchZoomed() ? '50%' : '640px',
      'padding-left': '9px',
      'position': 'relative',
    } as JSX.CSSProperties
  }

  const currentServerAddress = () => {
    return location.protocol + '//' + location.host.replace("3000", "5062")
  }

  const cast = () => {
    return async () => {
      await switchTwitchChannel(user().UserName)
    }
  }

  const headerBackground = (user: UserInfo) => {
    return `list-group-item ${user.WasBanned ? 'bg-red' : user.HasQuit ? 'bg-green' : 'bg-black'}`
  }

  return <table style="width: 100%">
    <tbody>
      <tr style="vertical-align: top">
        <td style="width: 1%; padding-right: 9px;"><MiniPlayerList /></td>
        <td>
          <ul class="list-group" style="padding-bottom: 20px">
            <li class={headerBackground(user())}>
              <div class="row">
                <div class="col">
                  <h1 style="color: white">{user().UserName} <span class="timer">〽️{displayValue(stats()!['Timestamp'], 'Timestamp')}</span></h1>
                </div>
                <div class="col" style="text-align: right">
                  <div class="castButton" onClick={cast()}>Stream anzeigen <b>{user().WasShownTimes}</b></div>
                </div>
              </div>
            </li>
            <li class="list-group-item"><SetDirectionInstruction userId={params.id} /></li>
          </ul>
          <Show when={stats()} fallback={<strong>Keine Daten verfügbar.</strong>}>
            <div>
              <For each={Details}>{(section, i) =>
                <div>
                  <div style="background-color: black; color: white; padding: 4px 8px 2px 8px">
                    <h4>{['Basics', 'Quests', 'Other'][i()]}</h4>
                  </div>
                  <div class="row align-items-start" style="padding-bottom: 20px">
                    <For each={section}>{(detail) =>
                      <div class="col">
                        <ul class="list-group">
                          <For each={detail}>{(col) =>
                            <li class="list-group-item bigger highlight" onClick={showGraph(col.id)}>
                              <span class="bigger" style={statsHeaderColor(col.id, displayValue(stats()![col.id], col.id))}>{col.displayName}</span><br />
                              <b>{displayValue(stats()![col.id], col.id) || '0'}</b>
                            </li>
                          }</For>
                        </ul>
                      </div>
                    }</For>
                  </div>
                </div>
              }</For>
            </div>
          </Show>
        </td>
        <td style={zoomColumn()}>
          <div id="twitch-embed" class="hdRatio" style="padding-bottom: 9px;"></div>
          <iframe class="hdRatio" style={{ 'background-color': 'black' }}
            src={`${currentServerAddress()}/api/embedgraph/${selectedStat()}/${user().UserName}/0/0`}
            scrolling="no"
          ></iframe>
          <div class="twitchzoom" onClick={() => twitchZoom()}>
            {twitchZoomed() ?
              <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" fill="currentColor" viewBox="0 0 15 15"> <path fill-rule="evenodd" d="M6.5 12a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11zM13 6.5a6.5 6.5 0 1 1-13 0 6.5 6.5 0 0 1 13 0z" /> <path d="M10.344 11.742c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1 6.538 6.538 0 0 1-1.398 1.4z" /> <path fill-rule="evenodd" d="M3 6.5a.5.5 0 0 1 .5-.5h6a.5.5 0 0 1 0 1h-6a.5.5 0 0 1-.5-.5z" /> </svg>
              :
              <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" fill="currentColor" viewBox="0 0 16 16"> <path fill-rule="evenodd" d="M6.5 12a5.5 5.5 0 1 0 0-11 5.5 5.5 0 0 0 0 11zM13 6.5a6.5 6.5 0 1 1-13 0 6.5 6.5 0 0 1 13 0z" /> <path d="M10.344 11.742c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1 6.538 6.538 0 0 1-1.398 1.4z" /> <path fill-rule="evenodd" d="M6.5 3a.5.5 0 0 1 .5.5V6h2.5a.5.5 0 0 1 0 1H7v2.5a.5.5 0 0 1-1 0V7H3.5a.5.5 0 0 1 0-1H6V3.5a.5.5 0 0 1 .5-.5z" /> </svg>
            }
          </div>
        </td>
      </tr>
    </tbody>
  </table>
}