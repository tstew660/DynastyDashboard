import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const WorstTeam = ({teams}) => {
    const isSuperFlex = useOutletContext();
    const worstTeam = teams.reduce(function(prev, curr) {
        return prev.ktc_total_sf < curr.ktc_total_sf ? prev : curr;
    });
    console.log(worstTeam);
    return(
        <div>
            <Card bg="dark">
                <Card.Title>Worst Dynasty Team</Card.Title>
                <Card.Text>
                    {worstTeam.user.metadata.team_name + " :("} 
                </Card.Text>
            </Card>
        </div>
    )

}