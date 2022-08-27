import { staticIntegration, useParams } from "@solidjs/router";
import { createMemo, For, Show, VoidComponent } from "solid-js";
import { LatestTable } from "./LatestTable";
import { useRimionship } from "./RimionshipContext";
import { CC, displayValue } from "./Utils";

const MiniColumns = [
  CC('Place', '#'),
  CC('UserId', 'Spieler')
];

const DetailColumns = [
  CC('Wealth', 'Koloniewert'),
  CC('AmountBloodCleaned', 'Blutsäuberung'),
  CC('AnimalMeatCreated', 'Tierfleischerzeugnisse'),
  CC('Caravans', 'Karnevalle'),
  CC('Colonists', 'Darmisten'),
  CC('Fire', 'Feuer!'),
  CC('Electricity', 'Stroom'),
  CC('Timestamp', 'Letztes Update'),
  CC('MedicalConditions', 'Medizinische Bedingungen')
]

const MiniPlayerList: VoidComponent = () => <LatestTable sortable={false} columns={MiniColumns} />;

export const PlayerDetail: VoidComponent = () => {
  const params = useParams<{ id: string }>();
  const { latestStats, users } = useRimionship();

  const user = createMemo(() => users[params.id]);
  const stats = createMemo(() => latestStats.find(s => s.UserId === params.id));

  return <div class="row">
    <div class="col-2">
      <MiniPlayerList />
    </div>
    <div class="col-5">
      <h1>{user().UserName}</h1>
      <Show when={stats()} fallback={<strong>Keine Daten verfügbar.</strong>}>
        <div class="row align-items-start">
          <For each={DetailColumns}>{(col) =>
            <div class="col-3">
              <dt>{col.displayName}</dt>
              <dd>{displayValue(stats()![col.id], col.id)}</dd>
            </div>
          }</For>
        </div>
      </Show>
    </div>
    <div class="col">
      <iframe
        src={`https://player.twitch.tv/?channel=${user().UserName}&parent=${encodeURIComponent(window.location.host)}}`}
        width="640"
        height="480"
        class="border border-1 border-primary"
        allowfullscreen>
      </iframe>
    </div>
  </div>
}