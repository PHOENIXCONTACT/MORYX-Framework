/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as chartjs from "chart.js";
import * as React from "react";
import { ChartData, Line } from "react-chartjs-2";
import SystemLoadSample from "../../common/models/SystemLoadSample";

interface CPUMemoryPropsModel {
    Samples: SystemLoadSample[];
}

interface CPUMemoryStateModel {
    chartData: any;
}

export class CPUMemoryChart extends React.Component<CPUMemoryPropsModel, CPUMemoryStateModel> {
    constructor(props: CPUMemoryPropsModel) {
        super(props);
        this.state = {
            chartData: CPUMemoryChart.createChartData(),
        };
    }

    public componentDidMount(): void {
        this.updateChartData(this.props);
    }

    public componentWillReceiveProps(nextProps: CPUMemoryPropsModel): void {
        this.updateChartData(nextProps);
    }

    public updateChartData(nextProps: CPUMemoryPropsModel): void {
        const chartData = CPUMemoryChart.createChartData();
        nextProps.Samples.forEach((sample) => {
            chartData.labels.push(sample.Date);
            chartData.datasets[0].data.push(sample.CPULoad);
            chartData.datasets[1].data.push(sample.SystemMemoryLoad);
        });

        this.setState({chartData});
    }

    private static createChartData(): any {
        const chartData: ChartData<chartjs.ChartData> = {
            labels: [],
            datasets:
            [
                {
                    label: "CPU load (%)",
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(218, 165, 32, 0.5)",
                    data: [],
                },
                {
                    label: "Used memory (%)",
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(0, 0, 255, 0.5)",
                    data: [],
                },
            ],
        };
        return chartData;
    }

    public render(): React.ReactNode {
        return (
            <Line  data={this.state.chartData} />
        );
    }
}
