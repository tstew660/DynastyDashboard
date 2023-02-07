import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const BestRedraftTeam = ({teams}) => {
    const isSuperFlex = useOutletContext();
    const bestTeam = teams.reduce(function(prev, curr) {
        return prev.fp_total_sf > curr.fp_total_sf ? prev : curr;
    });
    console.log(bestTeam);
    return(
        <div>
            <Card bg="dark">
                <Card.Title>Best Redraft Team</Card.Title>
                <Card.Text>
                    {bestTeam.user.metadata.team_name + "🏈"} 
                </Card.Text>
            </Card>
        </div>
    )

}