import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const WorstRedraftTeam = ({teams}) => {
    const isSuperFlex = useOutletContext();
    const worstTeam = teams.reduce(function(prev, curr) {
        return prev.fp_total_sf < curr.fp_total_sf ? prev : curr;
    });
    console.log(worstTeam);
    return(
        <div>
            <Card bg="dark">
                <Card.Title>Worst Redraft Team</Card.Title>
                <Card.Text>
                    {worstTeam.user.metadata.team_name + " :("} 
                </Card.Text>
            </Card>
        </div>
    )

}