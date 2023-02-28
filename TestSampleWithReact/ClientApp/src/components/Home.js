import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'

export class Home extends Component {
    constructor(props) {
        super(props);
        this.state = {
            currentUserId: "",
            isAdmin: false,
            users: [],
            loading: "noaccess"
        };
    }

    static displayName = Home.name;

    async componentDidMount() {
        const user = await authService.getUser();
        if (user)
            this.populateUserData();
    }

    renderUserTable(users, isAdmin) {
        return (
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                        <th>User Name</th>
                        <th>Avatar</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user =>
                        <tr key={user.userId}>
                            <td width="50%">{user.name}</td>
                            <td><img src={"data:image/*;base64," + user.avatar} alt="avatar" width="100" /></td>
                            <td>
                                {this.state.currentUserId !== user.userId && isAdmin
                                    ? <button onClick={() => this.deleteUser(user.userId)}>Delete</button>
                                    : null
                                }
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.getContent();

        return (
          <div>
                <h1>Hello, world!</h1>
                <p>Welcome to your new single-page application</p>
                {contents}
          </div>
        );
    }

    getContent() {
        switch (this.state.loading) {
            case "loaded":
                return this.renderUserTable(this.state.users, this.state.isAdmin);
            case "noaccess":
                return <p>No data available. Please login.</p>
            default:
                return <p><em>Loading...</em></p>
        }
    }

    async deleteUser(userId) {
        var result = window.confirm("Do you want to delete selected user?");
        if (result) {
            if (this.state.currentUserId === userId) {
                console.error('trying to delete yourself.')
                return;
            }

            const token = await authService.getAccessToken();
            fetch('api/v1/users/' + userId, {
                method: 'DELETE',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            })
                .then(() => {
                    this.populateUserData();
                });
        }
    }

    async populateUserData() {
        this.setState({ loading: "" })
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        fetch('api/v1/users', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        })
            .then(async (response) => {
                const data = await response.json();

                this.setState({ users: data.users, loading: "loaded", isAdmin: data.isAdmin, currentUserId: user.sub });
            })
            .catch(error => {
                console.error('There was an error!', error);
            });
    }
}
