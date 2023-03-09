import { useOutletContext } from "react-router-dom";
import { Card } from "react-bootstrap";

export const SellTheFarm = ({teams}) => {
    const isSuperFlex = useOutletContext();
    let maxDiff = 0;
    let team = "";
    teams.forEach(x => {
        if(x.ktc_total_sf - x.fp_total_sf > maxDiff) {
            maxDiff = x.ktc_total_sf - x.fp_total_sf;
            team = x;
        }
    });
    return(
        <div>
            <Card bg="dark">
                <Card.Title>Sell The Farm</Card.Title>
                <Card.Text>
                    {team.user.metadata.team_name + "💸"} 
                </Card.Text>
            </Card>
        </div>
    )

}