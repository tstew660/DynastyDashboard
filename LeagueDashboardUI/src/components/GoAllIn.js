import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const GoAllIn = ({teams}) => {
    const isSuperFlex = useOutletContext();
    let maxDiff = 0;
    let team = "";
    teams.forEach(x => {
        if(x.fp_total_sf - x.ktc_total_sf > maxDiff) {
            maxDiff = x.fp_total_sf - x.ktc_total_sf;
            team = x;
        }
    });
    return(
        <div>
            <Card bg="dark">
                <Card.Title>Go All In</Card.Title>
                <Card.Text>
                    {team.user.metadata.team_name + "🎰"} 
                </Card.Text>
            </Card>
        </div>
    )

}