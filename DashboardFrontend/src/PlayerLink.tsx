import { createMemo, Show, VoidComponent } from "solid-js";
import { useRimionship } from "./RimionshipContext";

export const PlayerLink : VoidComponent<{ id: string }> = (props) => {
  const { users } = useRimionship();

  const user = createMemo(() => users[props.id]);

  return <>
    <img src={user().AvatarUrl} class="mini-avatar" />
    {user().UserName}
  </>
};