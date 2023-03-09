import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const TeamHeader = ({teams, team}) => {
    const isSuperFlex = useOutletContext();
    let teamsRanked = teams.sort((a, b) => (a.ktc_total_sf > b.ktc_total_sf) ? -1 : 1);
    let rank = teamsRanked.indexOf(teamsRanked.find(x => {
       return x.roster_id == team.roster_id;
    })) + 1;
    return(
        <div>
            <Card bg="dark">
                <Card.Title>{team.user.metadata.team_name}</Card.Title>
                <Card.Text>
                    Team {rank} out of {teamsRanked.length}
                </Card.Text>
            </Card>
        </div>
    )

}