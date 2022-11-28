import React, { Component } from 'react';
import { GiLion, GiCow } from "react-icons/gi";
import './styles.css'

export class Ambiente extends Component {
    static displayName = Ambiente.name;

    constructor(props) {
        super(props);
        this.state = { ambiente: null, presaCapturada: false, isMovimentoAutomatico: false, tamanho: null, quantidadePresas: null, quantidadePredadores: null };
        this.movimentaAmbiente = this.movimentaAmbiente.bind(this);
        this.movimentaAutomatico = this.movimentaAutomatico.bind(this);
        this.iniciaAmbiente = this.iniciaAmbiente.bind(this);
        this.paraMovimentoAutomatico = this.paraMovimentoAutomatico.bind(this);
        this.renderInputDados = this.renderInputDados.bind(this);
        this.handleTamanho = this.handleTamanho.bind(this);
        this.handleQuantidadePresas = this.handleQuantidadePresas.bind(this);
        this.handleQuantidadePredadores = this.handleQuantidadePredadores.bind(this);

    }

    async movimentaAmbiente() {
        const response = await fetch('predadorpresa');

        const dados = await response.json();

        console.log(dados);

        if (dados[0] == null) {
            this.setState({
                presaCapturada: true,
                isMovimentoAutomatico: false
            });

            return;
        }

        this.setState({
            ambiente: dados
        });
    }

    renderizaAmbiente() {
        let ambiente = this.state.ambiente;

        console.log("ambiente", ambiente);

        if (ambiente === null)
            return null;

        let teste = ambiente.map(forecast => {
            return <div>

                {forecast.map(forecast2 => {
                    console.log("rara", forecast2);

                    let obj = JSON.parse(forecast2);
                    let background = "espacamento-posicao";

                    if (obj.Agente === null) {

                        let valor = '    ';

                        if (obj.Feromonio > 0) {
                            background = 'ambiente-amarelo'
                            valor = `  ${obj.Feromonio}  `;
                        }

                        return <div className={background}>{valor}</div>
                    }

                    if (obj.Agente.Tipo === 0) {
                        if (obj.Agente.Cor === 1)
                            background = 'ambiente-vermelho';
                        else
                            background = 'ambiente-azul';

                    } else {
                        if (obj.Agente.Modo === 1)
                            background = 'ambiente-laranja';
                    }

                    return <div className={background}>{obj.Agente.Tipo == 0 ? <GiCow /> : <GiLion />}</div>
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
        await this.setState({
            isMovimentoAutomatico: true
        });

        console.log("automático", this.state.isMovimentoAutomatico);
        while (this.state.isMovimentoAutomatico) {
            await new Promise(r => setTimeout(r, 200));
            await this.movimentaAmbiente();
        }
    }

    paraMovimentoAutomatico() {
        this.setState({
            isMovimentoAutomatico: false
        });
    }

    async iniciaAmbiente() {
        let tamanho = this.state.tamanho;
        let quantidadePresas = this.state.quantidadePresas;
        let quantidadePredadores = this.state.quantidadePredadores;

        let data = {
            Tamanho: tamanho,
            NumeroPresas: quantidadePresas,
            NumeroPredadores: quantidadePredadores

        };

        let response = await fetch('predadorpresa', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        const dados = await response.json();

        console.log("inicio", dados);

        this.setState({
            ambiente: dados,
            presaCapturada: false
        });
    }

    renderPresaCapturada() {
        let presaCapturada = this.state.presaCapturada;

        if (presaCapturada)
            return (<div>A presa foi capturada</div>)
        else
            return null;
    }

    handleTamanho(event) {
        this.setState({ tamanho: event.target.value });
    }

    handleQuantidadePresas(event) {
        this.setState({ quantidadePresas: event.target.value });
    }

    handleQuantidadePredadores(event) {
        this.setState({ quantidadePredadores: event.target.value });
    }

    renderInputDados() {
        let tamanho = this.state.tamanho;
        let quantidadePresas = this.state.quantidadePresas;
        let quantidadePredadores = this.state.quantidadePredadores;

        return (
            <div>
                <label className="inputs">
                    Tamanho:
                    <input type="number" value={tamanho} onChange={this.handleTamanho} />
                </label>
                <label className="inputs">
                    Quantidade de Presas:
                    <input type="number" value={quantidadePresas} onChange={this.handleQuantidadePresas} />
                </label>
                <label className="inputs">
                    Quantidade de Predadores:
                    <input type="number" value={quantidadePredadores} onChange={this.handleQuantidadePredadores} />
                </label>
            </div>);
    }

    render() {
        let isMovimentoAutomatico = this.state.isMovimentoAutomatico;

        return (
            <div>
                <h1>Ambiente</h1>

                <p>Sistema multiagentes predador-presa, onde a finalidade da predador é capturar a presa(Uma presa é capturada quando está cercada por 4 predadores).</p>

                {this.renderInputDados()}
                <div>{this.renderizaAmbiente()}</div>

                <div>{this.renderPresaCapturada()}</div>

                {!isMovimentoAutomatico && <button className="btn btn-primary" onClick={this.iniciaAmbiente}>Iniciar</button>}

                {!isMovimentoAutomatico && <button className="btn btn-primary" onClick={this.movimentaAmbiente}>Movimenta</button>}
                {!isMovimentoAutomatico && <button className="btn btn-primary" onClick={this.movimentaAutomatico}>Movimenta Automatico</button>}
                {isMovimentoAutomatico && <button className="btn btn-primary" onClick={this.paraMovimentoAutomatico}>Parar Movimento</button>}
            </div>
        );
    }
}
