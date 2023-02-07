import { Navbar, Container, Nav, Spinner, Form,  } from 'react-bootstrap';
import { Outlet, Link } from 'react-router-dom';
import { useState } from 'react';
import { useSelector } from 'react-redux';
import { useNavigation } from 'react-router-dom';


export function MainNavigation() {
  const [isSuperFlex, setIsSuperFlex] = useState(false);
  const stateValue = useSelector((state) => state.leagues.value);
  const navigation = useNavigation();

  console.log(stateValue.leagueId);

    function ScoringSwitch() {
        return (
          <Form className='text-light'>
            <Form.Check 
              type="switch"
              id="custom-switch"
              checked = {isSuperFlex}
              onChange = {e => setIsSuperFlex(e.target.checked)}
              label={isSuperFlex ? "SuperFlex" : " One QB"}
            />
          </Form>
        );
      }
    return (
    <div>
    <Navbar expand={false} bg="dark" variant="dark">
        <Container>
          <Link to="/">
            <Navbar.Brand href="/">Dynasty Dashboard</Navbar.Brand>
          </Link>
          < ScoringSwitch />
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto">
              <Link to="/">Home</Link>
              <Link to={`/overview/${stateValue.leagueId}`}>League Overview</Link>
              <Link to={`/team/${stateValue.leagueId}/${stateValue.teamId}`}>Team Overview</Link>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>
      <div class="mainContent">
      {navigation.state === "loading" ? <Spinner></Spinner> :
      <div>
      <h1 className='text-light'>{stateValue.leagueName}</h1>
      <Outlet context={isSuperFlex} />
      </div>}
      </div>
      </div>
    );
  }

  