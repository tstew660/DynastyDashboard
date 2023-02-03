import React from "react"
import { useLoaderData, useParams, useOutletContext  } from "react-router-dom"
import { Card, Row, Col, Container } from 'react-bootstrap';
import { useDispatch } from "react-redux";
import { updateTeam } from "./Leagues";
import { Form } from "react-bootstrap";
import { useState } from "react";

export default function TeamOverviewComponent() {
    const dispatch = useDispatch();
    const isSuperFlex = useOutletContext();
    const params = useParams();
    const leagueObject = useLoaderData();
    const rostersObject = leagueObject.rosters;
    const [teamId, setTeamId] = useState(params.teamId);
    const team = rostersObject.find((roster) => {
        return roster.roster_id == teamId;
    });
    const qbPositions = leagueObject.roster_positions.filter((position) =>{
        return position === "QB" 
    });
    const rbPositions = leagueObject.roster_positions.filter((position) =>{
        return position === "RB"
    });
    const wrPositions = leagueObject.roster_positions.filter((position) =>{
        return position === "WR"
    });
    const tePositions = leagueObject.roster_positions.filter((position) =>{
        return position === "TE"
    });
    const flexPositions = leagueObject.roster_positions.filter((position) =>{
        return position === "FLEX"
    });
    const superFlexPositions = leagueObject.roster_positions.filter((position) =>{
        return position === "SUPER_FLEX"
    });
    
    
    let teamList = team.playersList.filter(player => {
        return player.position === "qb" || player.position === "rb" || player.position === "wr" || player.position === "te"
    });
    const rankedPositions = numberedPositions(qbPositions, rbPositions, wrPositions, tePositions, flexPositions, superFlexPositions);
    teamList.playersList = applyPositionTier(teamList, qbPositions, rbPositions, wrPositions, tePositions, flexPositions, superFlexPositions);
    console.log(teamList);

    console.log(rankedPositions);
    function applyPositionTier(teamList, qbPositions, rbPositions, wrPositions, tePositions, flexPositions, superFlexPositions) {

        const numOfQbPositions = qbPositions.length
        const numOfRbPositions = rbPositions.length
        const numOfWrPositions = wrPositions.length
        const numOfTePositions = tePositions.length
        const numOfFlexPositions = flexPositions.length;
        const numOfSfPositions = superFlexPositions.length;
        let qbCounter = 0;
        let rbCounter = 0;
        let wrCounter = 0;
        let teCounter = 0;
        let flexCounter = 0;
        let sfCounter = 0;
        let sortedTeamList = isSuperFlex ? teamList.sort((a, b) => (a.fantasy_pros_rank_sf > b.fantasy_pros_rank_sf) ? -1 : 1)
        : teamList.sort((a, b) => (a.fantasy_pros_rank_oneQB > b.fantasy_pros_rank_oneQB) ? -1 : 1)

        sortedTeamList.forEach(player => {

            if(player.position === "qb") {
                if(qbCounter < numOfQbPositions) {
                    player.lineupRole = "QB" + (qbCounter + 1);
                    qbCounter++; 
                    if(qbCounter >= numOfQbPositions) {
                        sfCounter = 0;
                    }   
                }
                else {
                    player.lineupRole = "SF" + (sfCounter + 1);
                    sfCounter++;
                    if(sfCounter >= numOfSfPositions) {
                        qbCounter = 0;
                    }   
                } 
            }
            if(player.position === "rb") {
                if(rbCounter < numOfRbPositions) {
                    player.lineupRole = "RB" + (rbCounter + 1);
                    rbCounter++;
                    if(rbCounter >= numOfRbPositions) {
                        flexCounter = 0;
                    } 
                }
                else {
                    player.lineupRole = "FLEX" + (flexCounter + 1 );
                    flexCounter++;
                    if(flexCounter >= numOfFlexPositions) {
                        wrCounter = 0;
                        teCounter = 0;
                        rbCounter = 0;
                    } 
                } 
            }
            if(player.position === "wr") {
                if(wrCounter < numOfWrPositions) {
                    player.lineupRole = "WR" + (wrCounter + 1);
                    wrCounter++;
                    if(wrCounter >= numOfWrPositions) {
                        flexCounter = 0;
                    } 
                }
                else {
                    player.lineupRole = "FLEX" + (flexCounter + 1);
                    flexCounter++;
                    if(flexCounter >= numOfFlexPositions) {
                        wrCounter = 0;
                        teCounter = 0;
                        rbCounter = 0;
                    } 
                } 
            }
            if(player.position === "te") {
                if(teCounter < numOfTePositions) {
                    player.lineupRole = "TE" + (teCounter + 1);
                    teCounter++;
                    if(teCounter >= numOfTePositions) {
                        flexCounter = 0;
                    } 
                }
                else {
                    player.lineupRole = "FLEX" + (flexCounter + 1);
                    flexCounter++;
                    if(flexCounter >= numOfFlexPositions) {
                        wrCounter = 0;
                        teCounter = 0;
                        rbCounter = 0;
                    } 
                } 
            }

        });
        return sortedTeamList;
        
    }
    


    function numberedPositions(qbPositions, rbPositions, wrPositions, tePositions, flexPositions, superFlexPositions)  {
        
        const positionDictionary = [];
        

        for (let i = 0; i < qbPositions.length; i++) {
            positionDictionary.push({position: "QB", number: i+1, chunks: qbPositions.length + superFlexPositions.length });
        }
        for (let i = 0; i < rbPositions.length; i++) {
            positionDictionary.push({position: "RB", number: i+1, chunks: rbPositions.length + flexPositions.length + superFlexPositions.length });
        }
        for (let i = 0; i < wrPositions.length; i++) {
            positionDictionary.push({position: "WR", number: i+1, chunks: wrPositions.length + flexPositions.length + superFlexPositions.length });
        }
        for (let i = 0; i < tePositions.length; i++) {
            positionDictionary.push({position: "TE", number: i+1, chunks: tePositions.length + flexPositions.length + superFlexPositions.length});
        }
        for (let i = 0; i < flexPositions.length; i++) {
            positionDictionary.push({position: "FLEX", number: (i+1), chunks: flexPositions.length + rbPositions.length + wrPositions.length + tePositions.length });
        }
        for (let i = 0; i < superFlexPositions.length; i++) {
            positionDictionary.push({position: "SF", number: (i+1), chunks: superFlexPositions.length + rbPositions.length + wrPositions.length + tePositions.length });
        }
        return positionDictionary;
    }

    function PlayerCard({players, position}) {
        let filteredList = [];
        console.log(players);
        console.log(position);
        filteredList = players.filter((player) => {
            return player.lineupRole.includes(position.position) && player.lineupRole.charAt(player.lineupRole.length - 1) == position.number;
        })
        

        return (
          <Card style={{ width: '18rem' }}>
            <Card.Body>
              <Card.Title>{position.position}</Card.Title>
              <Card.Text>
                {filteredList.map((player) => 
                    isSuperFlex ?
                    <p>{player.first_name + " " + player.last_name + " (" + player.fantasy_pros_rank_sf +")"}</p> :
                    <p>{player.first_name + " " + player.last_name + " (" + player.fantasy_pros_rank_oneQB +")"}</p>
                )}
              </Card.Text>
            </Card.Body>
          </Card>
        );
      }

    let handleTeamChange = (e) => {
        const teamId = e.target.value;
        dispatch(updateTeam(teamId));
        setTeamId(teamId);
      }

    return (
        <Container>
            <Row className="justify-content-md-center mb-4" xs="auto">
                <h1>{team.user.metadata.team_name}</h1>
            </Row>
            <Row className="mb-4" xs="auto">
                <Form.Select onChange={handleTeamChange}>
                    <option value=""> -- Select a Team -- </option>
                    {rostersObject && rostersObject?.map((user) => <option value={user.roster_id} key={user.roster_id}>{user.user.metadata.team_name}</option>)}
                </Form.Select>
            </Row>
            <Row className="mb-4" xs="auto">
                <Container>
                    <Row className="mb-4">
                        {rankedPositions.map((position) => 
                        <Col >
                            < PlayerCard players={teamList} position={position} />
                        </Col>)}
                    </Row>
                </Container>
            </Row>
        </Container>
    )
  }