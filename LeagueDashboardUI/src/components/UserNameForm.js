import { Form, Button, Row, Col, Container, Spinner } from 'react-bootstrap';
import { useForm } from 'react-hook-form';
import { GetData } from './ApiAccess.js'
import { useState } from 'react'
import { useNavigate, useOutletContext } from 'react-router-dom';
import { updateLeague } from './Leagues.js';
import { useDispatch } from 'react-redux';
import { ApiEndpoint } from '../ApiConfig.js';

export function UserNameform() {

    const dispatch = useDispatch();
    const [userModel, setUser] = useState([]);
    const [showSpinner, setShowSpinner] = useState(false);
    const [leagueLoaded, setLeagueLoaded] = useState(false);
    let navigate = useNavigate();

    const { register, handleSubmit, formState: { errors } } = useForm();

    const onSubmit = data => {
        setShowSpinner(true);
        GetData(`${ApiEndpoint}/User/${data.UserName}`)
        .then((userResponse) => setUser(userResponse))
        .finally(() => {
            setShowSpinner(false);
            setLeagueLoaded(true)})

    }

    let handleLeagueChange = (e) => {
        const leagueId = e.target.value;
        const leagueName = userModel.leagues.find((x) => {
            return x.league_id == leagueId}).name;
        dispatch(updateLeague({leagueId: leagueId, leagueName: leagueName }));
        navigate("/overview/" + leagueId);
      }

      

    return (
        <Container className="text-light" bg="dark">
            <Row className="justify-content-md-center mb-4" xs="auto">
                <Col>
                    <div>Welcome to Dynasty Dashboard! Please enter your Sleeper Username below to get started.</div>
                </Col>
            </Row>
            <Row className="" xs="auto"> 
                <Col sm>
                    <Form className="mb-4" onSubmit={handleSubmit(onSubmit)}>
                        <Row>
                            <Col>
                                <Form.Group>
                                    <Form.Control {...register('UserName')} placeholder="Please Enter your Sleeper Username" />
                                    {errors.UserName && <p>UserName is required.</p>}
                                </Form.Group>
                            </Col>
                            <Col xs="auto">
                                <Button class="btn btn-primary mb-2" variant="secondary" type="submit">Submit</Button>
                            </Col>
                        </Row>
                    </Form>
                </Col>
            </Row>
            {leagueLoaded ?
            showSpinner ? <Spinner></Spinner> :
            <Row className="" xs="auto" >
                <Col sm>
                    <Form className="mb-4" variant="dark" onChange={handleLeagueChange}>
                    <Form.Select variant="dark">
                        <option value=""> -- Select a League -- </option>
                        {userModel && userModel?.leagues?.map((selectedLeague) => <option value={selectedLeague.league_id} key={selectedLeague.league_id} name={selectedLeague.name} >{selectedLeague.name}</option>)}
                    </Form.Select>
                    </Form>
                </Col>
            </Row>
        :
        <div></div>}
        </Container>
    )
}
