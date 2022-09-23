import { useParams } from "@solidjs/router"
import { JSX, onMount } from "solid-js"
import { createEffect, createMemo, createSignal, For, Show, VoidComponent } from "solid-js"
import { LatestTable } from "./LatestTable"
import { useRimionship } from "./RimionshipContext"
import { CC, displayValue } from "./Utils"
import { UserInfo } from "./MessageDTOs"

const MiniColumns = [
  CC('Place', '#'),
  CC('UserId', 'Spieler')
]

const Details = [
  [
    [
      CC('Wealth', 'Koloniereichtum'),
      CC('Enemies', 'Feinde'),
      CC('NumThreatBigs', 'Große Bedrohungen'),
    ], [
      CC('MapCount', 'Anzahl Karten'),
      CC('MentalColonists', 'Durchgedrehte Kolonisten'),
      CC('Fire', 'Brandstellen'),
    ], [
      CC('Colonists', 'Anzahl Kolonisten'),
      CC('DownedColonists', 'Ohnmächtige Kolonisten'),
      CC('WeaponDps', 'Waffenkraft'),
    ], [
      CC('ColonistsNeedTending', 'Verletzte Kolonisten'),
      CC('Conditions', 'Wetter & Kartenbedingungen'),
      CC('Electricity', 'Verfügbare Energie'),
    ], [
      CC('MedicalConditions', 'Krankeiten'),
      CC('NumRaidsEnemy', 'Feindliche Überfälle'),
      CC('Prisoners', 'Gefangene'),
    ],
  ], [
    [
      CC('TicksIgnoringBloodGod', 'Zeit mit Ignoranz des Blutgottes'),
    ], [
      CC('TicksLowColonistMood', 'Zeit mit niedriger Moral'),
    ], [
      CC('ColonistsKilled', 'Kolonisten getötet'),
    ], [
      CC('AmountBloodCleaned', 'Menge aufgewischtes Blut'),
    ], [
      CC('AnimalMeatCreated', 'Geschlachtetes Tierfleisch'),
    ],
  ], [
    [
      CC('Rooms', 'Bebaute Fläche'),
      CC('WildAnimals', 'Wilde Tiere'),
      CC('DamageTakenPawns', 'Schaden an Kolonisten'),
    ], [
      CC('Medicine', 'Medizin'),
      CC('TamedAnimals', 'Gezähmte Tiere'),
      CC('DamageTakenThings', 'Schaden an der Kolonie'),
    ], [
      CC('Food', 'Essen'),
      CC('Visitors', 'Besucher'),
      CC('DamageDealt', 'Ausgeteilter Schaden'),
    ], [
      CC('GreatestPopulation', 'Maximale Anzahl Kolonisten'),
      CC('Temperature', 'Temperatur auf Hauptkarte'),
    ], [
      CC('Caravans', 'Kolonisten in Karawanen'),
      CC('InGameHours', 'Gespielte Stunden'),
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

  const user = createMemo(() => users[params.id])
  const stats = createMemo(() => latestStats.find(s => s.UserId === params.id))
  const [selectedStat, setSelectedStat] = createSignal(getStoredStat())

  var twitchPlayer: any = null

  createEffect((prevUser) => {
    if (!twitchPlayer)
      twitchPlayer = new Twitch.Embed("twitch-embed", {
        width: 640,
        height: 320,
        layout: 'video',
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

  return <div class="row">
    <div class="col" style="flex-grow: 0">
      <MiniPlayerList />
    </div>
    <div class="col" style="flex-grow: 9">
      <ul class="list-group" style="padding-bottom: 20px">
        <li class={headerBackground(user())}>
          <div class="row">
            <div class="col">
              <h1 style="color: white">{user().UserName} <span class="timer">〽️{displayValue(stats()!['Timestamp'], 'Timestamp')}</span></h1>
            </div>
            <div class="col" style="text-align: right">
              <button class="btn castButton" onClick={cast()}>Stream anzeigen</button>
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
    </div>
    <div class="col" style="flex-grow: 0">
      <div id="twitch-embed" class="border border-1 border-primary"></div>
      <iframe
        src={`${currentServerAddress()}/api/embedgraph/${selectedStat()}/${user().UserName}/620/340`}
        width="640"
        height="320"
        scrolling="no"
        style={{ 'background-color': 'black' }}>
      </iframe>
    </div>
  </div>
}