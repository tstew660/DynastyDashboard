import { Table, Container, Row, Nav, Pagination, Spinner} from 'react-bootstrap';
import { useOutletContext } from 'react-router-dom';
import { useLoaderData } from 'react-router-dom';
import { useState } from 'react';
import { GetData } from './ApiAccess';
import { ApiEndpoint } from '../ApiConfig';

export function PlayersTable() {
    const isSuperFlex = useOutletContext();
    const players = useLoaderData();
    const endpoint = ApiEndpoint();

    const [playerList, setPlayerList] = useState(players.players);
    const [activePage, setActivePage] = useState(1);
    const [showSpinner, setShowSpinner] = useState(false);

    const changePage = (e) => {
        const selectedPage = e
        console.log(e);
        setActivePage(selectedPage);
        setShowSpinner(true);
        GetData(`${endpoint}/players/GetAllPlayersAsync?pageNumber=${selectedPage}&pageSize=25`)
        .then((playerList) => setPlayerList(playerList.players))
        .finally(() => {
            setShowSpinner(false);})
    }

    function nameCase(string){
        return string[0].toUpperCase() + string.slice(1).toLowerCase();
      }
    

    return(
        <Container>
            <Row className="mb-4">
                        <Table striped bordered hover variant="dark" responsive className='player-table table'>
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Position</th>
                                    <th>Team</th>
                                    <th>Dynasty Value</th>
                                    <th>Redraft Value</th>
                                </tr>
                            </thead>
                            <tbody>
                                {playerList && playerList?.map((x) =>
                                    <tr>
                                        <td>{x.position != "PICK" && x.first_name != null && x.last_name != null ? nameCase(x.first_name) + " " + nameCase(x.last_name) : x.first_name + " " + x.last_name}</td>
                                        <td>{x.position != null ? x.position.toUpperCase() : "N/A"}</td>
                                        <td>{x.team != null ? x.team.toUpperCase() : "N/A"}</td>
                                        <td>{isSuperFlex ? x.ktc_rank_sf : x.ktc_rank_oneQB}</td>
                                        <td>{isSuperFlex ? x.fantasy_pros_rank_sf : x.fantasy_pros_rank_oneQB}</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    {showSpinner ? <Spinner className='spinner overlay' /> : <div></div>}
            </Row>
            <Row>
                <Pagination className="text-black">
                    <Pagination.First onClick={() => changePage(1)} key={1} />
                    <Pagination.Prev onClick={() => changePage(activePage - 1)} key={activePage - 1} />
                    <Pagination.Item>{activePage == 1 ? activePage : (activePage - 1) * 25 + 1} - {activePage == 1 ? activePage + 24 : (activePage - 1) * 25 + 25}</Pagination.Item>
                    <Pagination.Next onClick={() => changePage(activePage + 1)} key={activePage + 1} />
                    <Pagination.Last onClick={() => changePage(players.playersSize)} key={players.playersSize} />
                </Pagination>
            </Row>
        </Container>);
}