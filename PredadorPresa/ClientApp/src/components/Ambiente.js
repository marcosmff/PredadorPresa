import React, { Component } from 'react';
import { GiLion, GiCow } from "react-icons/gi";
import './styles.css'

export class Ambiente extends Component {
    static displayName = Ambiente.name;

    constructor(props) {
        super(props);
        this.state = { ambiente: null };
        this.movimentaAmbiente = this.movimentaAmbiente.bind(this);
        this.movimentaAutomatico = this.movimentaAutomatico.bind(this);
    }

    async movimentaAmbiente() {
        const response = await fetch('predadorpresa');

        const dados = await response.json();

        console.log(dados);


        this.setState({
            ambiente: dados
        });
    }

    renderizaAmbiente() {
        let ambiente = this.state.ambiente;

        console.log("ambiente", ambiente);

        if (ambiente == null)
            return null;

        let teste = ambiente.map(forecast => {
            return <div>

                {forecast.map(forecast2 => {
                    console.log("rara", forecast2);

                    let obj = JSON.parse(forecast2);

                    if (obj.Agente == null)
                        return <div> O </div>

                    return <div>{obj.Agente.Tipo == 0 ? <GiCow /> : <GiLion/>}</div>
                })}
                <br />
            </div>
        }

        )

        console.log("componentes", teste);

        return (
            <div className='ambiente-posicao'>
                {
                    teste
                }
            </div>);
    }

    async movimentaAutomatico() {
        while (true) {
            await new Promise(r => setTimeout(r, 500));
            await this.movimentaAmbiente();
        }
    }

    render() {
        return (
            <div>
                <h1>Ambiente</h1>

                <p>This is a simple example of a React component.</p>
                <p>Testaeeeeeasadasdsadsadasasdasdasdsa.</p>

                <div>{this.renderizaAmbiente()}</div>

                <button className="btn btn-primary" onClick={this.movimentaAmbiente}>Movimenta</button>
                <button className="btn btn-primary" onClick={this.movimentaAutomatico}>Movimenta Automatico</button>
            </div>
        );
    }
}
