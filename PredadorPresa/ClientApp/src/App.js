import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Ambiente } from './components/Ambiente';
import { Container } from 'reactstrap';

import './custom.css'

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Container>
                <Route exact path='/' component={Ambiente} />
            </Container>
        );
    }
}
