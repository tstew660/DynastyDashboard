export default function DraftPicks({team}) {
    return (
        <div>
            {team.picks.map((pick) => 
                    <div>{pick.season + " - " + pick.round + " (" + pick.rank_sf + ")"}</div>)}
        </div>
    )
}