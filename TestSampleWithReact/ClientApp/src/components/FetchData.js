import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import { Chart } from 'chart.js/auto'
import { Line } from 'react-chartjs-2'
import CalendarHeatmap from 'react-calendar-heatmap';
import 'react-calendar-heatmap/'

export class FetchData extends Component {

  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = {
      forecasts: [],
      loading: true,
      startDate: Date.now(),
      heatmapValues: [],
      endDate: Date.now()
    };

    this.onClick = this.onClick.bind(this);
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(forecasts) {
    return (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map(forecast =>
            <tr key={forecast.date}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }


  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.forecasts);
    let chartContents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.drawChart(this.state.forecasts)
    let headmapContents = this.state.loading
      ? <p><em>Please wait...</em></p>
      : this.drawCalendarHeatmap()

    return (
      <div className="row">
        <h1 id="tableLabel">Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
        {chartContents}
        {headmapContents}
      </div>
    );
  }

  onClick(value) {
    console.log(value);
  }

  drawCalendarHeatmap() {
    return (
      <div className="row" className="react-calendar-heatmap">
        <CalendarHeatmap
          endDate={this.state.endDate}
          startDate={this.state.startDate}
          values={this.state.heatmapValues}
          onClick={this.onClick}
          classForValue={(value) => {
            if (!value) {
              return 'color-empty';
            }
            switch (true) {
              case value.temperature <= -30:
                return 'color-github-0'
              case value.temperature > -30 && value.temperature <= 0:
                return 'color-github-0'
              case value.temperature > 0 && value.temperature <= 10:
                return 'color-github-0'
              case value.temperature > 10 && value.temperature <= 18:
                return 'color-github-0'
              case value.temperature > 18 && value.temperature <= 25:
                return 'color-github-0'
              case value.temperature > 25 && value.temperature <= 30:
                return 'color-github-0'
              case value.temperature > 30:
                return 'color-github-0'
            };

            return 'color-filled';
          }}
          />
      </div>
    );
  }

  drawChart(forecasts) {
    const values = [];
    forecasts.map(forecast => values.push(forecast.temperatureC));

    const data = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
      datasets: [
        {
          label: 'Dataset of Months',
          fill: false,
          lineTension: 0.1,
          backgroundColor: 'rgba(75,192,192,0.4)',
          borderColor: 'rgba(75,192,192,1)',
          borderCapStyle: 'butt',
          borderDash: [],
          borderDashOffset: 0.0,
          borderJoinStyle: 'miter',
          pointBorderColor: 'rgba(75,192,192,1)',
          pointBackgroundColor: '#fff',
          pointBorderWidth: 1,
          pointHoverRadius: 5,
          pointHoverBackgroundColor: 'rgba(75,192,192,1)',
          pointHoverBorderColor: 'rgba(220,220,220,1)',
          pointHoverBorderWidth: 2,
          pointRadius: 1,
          pointHitRadius: 10,
          data: values
        }
      ]
    };

    return (
      <div className="row">
        <Line data={data} />
      </div>
    );
  }

  async populateWeatherData() {
    const token = await authService.getAccessToken();
    const user = await authService.getUser();
    const response = await fetch('api/v1/weatherforecast/' + user.sub, {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();

    const heatmaps = data.map(item => ({ 'date': item.date, 'temperature': item.temperatureC }));

    const maxDate = new Date(Math.max.apply(null, heatmaps.map(it => new Date(it.date))));
    const minDate = new Date(Math.min.apply(null, heatmaps.map(it => new Date(it.date))));

    this.setState({ forecasts: data, loading: false, heatmapValues: heatmaps, endDate: maxDate.setMonth(maxDate.getMonth() + 1), startDate: minDate.setMonth(minDate.getMonth() - 1) });
  }
}
