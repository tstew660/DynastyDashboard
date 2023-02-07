import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const BestTeam = ({teams}) => {
    const isSuperFlex = useOutletContext();
    const bestTeam = teams.reduce(function(prev, curr) {
        return prev.ktc_total_sf > curr.ktc_total_sf ? prev : curr;
    });
    console.log(bestTeam);
    return(
        <div>
            <Card bg="dark">
                <Card.Title>Best Dynasty Team</Card.Title>
                <Card.Text>
                    {bestTeam.user.metadata.team_name + "🏆"} 
                </Card.Text>
            </Card>
        </div>
    )

}