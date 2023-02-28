import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'

export class Home extends Component {
    constructor(props) {
        super(props);
        this.state = {
            isAdmin: false,
            users: [],
            loading: true
        };
    }

    static displayName = Home.name;

    componentDidMount() {
        this.populateUserData();
    }

    static renderUserTable(users, isAdmin) {
        return (
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                        <th>User Name</th>
                        <th>Avatar</th>
                        {isAdmin ? <th>Actions</th> : null}
                    </tr>
                </thead>
                <tbody>
                    {users.map(user =>
                        <tr key={user.userId}>
                            <td width="50%">{user.name}</td>
                            <td><img src={"data:image/*;base64," + user.avatar} alt="avatar" className="img-thumbnail" /></td>
                            {isAdmin ? <td>Actions</td> : null}
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Home.renderUserTable(this.state.users, false);
    return (
      <div>
            <h1>Hello, world!</h1>
            <p>Welcome to your new single-page application</p>
            {contents}
      </div>
    );
    }

    async populateUserData() {
        const token = await authService.getAccessToken();
        const response = await fetch('api/v1/users', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();

        this.setState({ users: data.users, loading: false, isAdmin: data.isAdmin });
    }
}
