import { Link } from "@solidjs/router"
import { createMemo, VoidComponent } from "solid-js"
import { useRimionship } from "./RimionshipContext"

export const PlayerLink: VoidComponent<{ id: string }> = (props) => {
  const { users } = useRimionship()

  const user = createMemo(() => users[props.id])

  return <Link href={`/player/${props.id}`}>
    <span style="white-space: nowrap"><img src={user().AvatarUrl} class="mini-avatar" /> {user().UserName}</span>
  </Link>
}