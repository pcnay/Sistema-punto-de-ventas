using LinqToDB;
using Models;
using Models.Conexion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewModels.Library;

namespace ViewModels
{
	public class ClientesVM : Conexion
	{
		private List<TextBox> _textBoxCliente;
		private List<Label> _labelCliente;
		private TextBoxEvent evento;
		private string _accion = "insert";
		private PictureBox _imagePictureBox;
		private CheckBox _checkBoxCredito;
		private Bitmap _imagBitmap;
		private static DataGridView _dataGridView1;
		private int _reg_por_pagina = 10, _num_pagina = 1;



		public ClientesVM(object[] objectos, List<TextBox> textBoxCliente, List<Label> labelCliente)
		{
			_textBoxCliente = textBoxCliente;
			_labelCliente = labelCliente;
			_imagePictureBox = (PictureBox)objectos[0];
			_checkBoxCredito = (CheckBox)objectos[1];
			_imagBitmap = (Bitmap)objectos[2];
			_dataGridView1 = (DataGridView)objectos[3];
			evento = new TextBoxEvent();
			restablecer();
		}
		public void guardarCliente()
		{
			if (_textBoxCliente[0].Text.Equals(""))
			{
				_labelCliente[0].Text = "Este campo es requerido";
				_labelCliente[0].ForeColor = Color.Red;
				_textBoxCliente[0].Focus();
			}
			else
			{
				if (_textBoxCliente[1].Text.Equals(""))
				{
					_labelCliente[1].Text = "Este campo es requerido";
					_labelCliente[1].ForeColor = Color.Red;
					_textBoxCliente[1].Focus();
				}
				else
				{
					if (_textBoxCliente[2].Text.Equals(""))
					{
						_labelCliente[2].Text = "Este campo es requerido";
						_labelCliente[2].ForeColor = Color.Red;
						_textBoxCliente[2].Focus();
					}
					else
					{
						if (_textBoxCliente[3].Text.Equals(""))
						{
							_labelCliente[3].Text = "Este campo es requerido";
							_labelCliente[3].ForeColor = Color.Red;
							_textBoxCliente[3].Focus();
						}
						else
						{
							if (evento.comprobarFormatoEmail(_textBoxCliente[3].Text))
							{
								if (_textBoxCliente[4].Text.Equals(""))
								{
									_labelCliente[4].Text = "Este campo es requerido";
									_labelCliente[4].ForeColor = Color.Red;
									_textBoxCliente[4].Focus();
								}
								else
								{
									if (_textBoxCliente[5].Text.Equals(""))
									{
										_labelCliente[5].Text = "Este campo es requerido";
										_labelCliente[5].ForeColor = Color.Red;
										_textBoxCliente[5].Focus();
									}
									else
									{
										// Se realiza una consulta de tipo "Where", verifica si esta registrado el ID, Email del usuario
										//var cliente = TClientes.Where(c => c.Nid.Equals(_textBoxCliente[0].Text) ||
										//c.Email.Equals(_textBoxCliente[3].Text)).ToList();
										var cliente1 = TClientes.Where(p => p.Nid.Equals(_textBoxCliente[0].Text)).ToList();
										var cliente2 = TClientes.Where(p => p.Email.Equals(_textBoxCliente[3].Text)).ToList();
										var list = cliente1.Union(cliente2).ToList();
										switch (_accion)
										{
											case "insert":
												if (list.Count.Equals(0))
												{
													// Insertar datos en la tabla
													SaveData();
												}
												else
												{
													if (0 < cliente1.Count)
													{
														_labelCliente[0].Text = "El NID ya esta registrado";
														_labelCliente[0].ForeColor = Color.Red;
														_textBoxCliente[0].Focus();
													}
													if (0 < cliente2.Count)
													{
														_labelCliente[3].Text = "El Email ya esta registrado";
														_labelCliente[3].ForeColor = Color.Red;
														_textBoxCliente[3].Focus();
													}
												}
												break;
										}

									}
								}
							}
							else
							{
								_labelCliente[3].Text = "Email invalido";
								_labelCliente[3].ForeColor = Color.Red;
								_textBoxCliente[3].Focus();
							}

						}
					}
				}
			}
		}
		// Para guardar la información del cliente.
		private void SaveData()
		{
			BeginTransactionAsync();
			try
			{
				var srcImage = Objects.uploadimage.ResizeImage(_imagePictureBox.Image, 165, 100);
				var image = Objects.uploadimage.ImageToByte(srcImage);
				switch (_accion)
				{
					// Insertando datos en la tabla de Clientes.
					case "insert":
						TClientes.Value(c => c.Nid, _textBoxCliente[0].Text)
								.Value(u => u.Nombre, _textBoxCliente[1].Text)
								.Value(u => u.Apellido, _textBoxCliente[2].Text)
								.Value(u => u.Email, _textBoxCliente[3].Text)
								.Value(u => u.Telefono, _textBoxCliente[4].Text)
								.Value(u => u.Direccion, _textBoxCliente[5].Text)
								.Value(u => u.Credito, _checkBoxCredito.Checked)
								.Value(u => u.Fecha, DateTime.Now.ToString("dd/MMM/yyyy"))
								.Value(u => u.Imagen, image)
								.Insert();
								// Para obtener el último elementos de la colección.
								var cliente = TClientes.ToList().Last();

								// Insertando los datos a la Tabla de Reportes de Clientes.
								TReportes_clientes.Value(u => u.UltimoPago, 0)
										.Value(u => u.FechaPago, "--/--/--")
										.Value(u => u.DeudaActual, 0)
										.Value(u => u.FechaDeuda, "--/--/--")
										.Value(u => u.FechaLimite, "--/--/--")
										.Value(u => u.Ticket, "0000000000")
										.Value(u => u.IdCliente, cliente.ID)
										.Insert();
						break;

				}
				CommitTransaction(); // Para grabar los datos a las Tablas.
				restablecer();
			}
			catch (Exception ex)
			{
				RollbackTransaction();
				MessageBox.Show(ex.Message);
			}
		}

		// Se utiliza de forma Asincronica, ya va terminando las etapas para ir avanzando
		/* 
		 Funcion normal
			public void SearchClientes(string campo)
			{
				// Filtrando a los clientes.
				List<TClientes> query;
				int inicio = (_num_pagina - 1) * _reg_por_pagina;
				if(campo.Equals(""))
				{
					query = TClientes.ToList();
				}
			}

		 */
		public async Task SearchClientesAsync(string campo)
		{
			// Filtrando a los clientes.
			List<TClientes> query;
			int inicio = (_num_pagina - 1) * _reg_por_pagina;
			if(campo.Equals(""))
			{
				query = await TClientes.ToListAsync();
			}
			else
			{
				query = await TClientes.Where(c => c.Nid.StartsWith(campo) || c.Nombre.StartsWith(campo) || c.Apellido.StartsWith(campo)).ToListAsync();
			}
			// En esta parte se asigna los datos al Grid de la pantalla en Captura de Clientes.
			_dataGridView1.DataSource = query.Skip(inicio).Take(_reg_por_pagina).ToList();
			// Ocultando algunas columnas.
			_dataGridView1.Columns[0].Visible = false;
			_dataGridView1.Columns[7].Visible = false;
			_dataGridView1.Columns[9].Visible = false;
			_dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.WhiteSmoke;
			_dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.WhiteSmoke;
			_dataGridView1.Columns[5].DefaultCellStyle.BackColor = Color.WhiteSmoke;
			_dataGridView1.Columns[7].DefaultCellStyle.BackColor = Color.WhiteSmoke;

		}

		// Asigna los valores a los campos de captura de Clientes.
		private int _idCliente = 0;
		public void GetCliente()
		{
			_accion = "update";
			_idCliente = Convert.ToInt16(_dataGridView1.CurrentRow.Cells[0].Value);
			_textBoxCliente[0].Text = Convert.ToString(_dataGridView1.CurrentRow.Cells[1].Value);
			_textBoxCliente[1].Text = Convert.ToString(_dataGridView1.CurrentRow.Cells[2].Value);
			_textBoxCliente[2].Text = Convert.ToString(_dataGridView1.CurrentRow.Cells[3].Value);
			_textBoxCliente[3].Text = Convert.ToString(_dataGridView1.CurrentRow.Cells[4].Value);
			_textBoxCliente[4].Text = Convert.ToString(_dataGridView1.CurrentRow.Cells[6].Value);
			_textBoxCliente[5].Text = Convert.ToString(_dataGridView1.CurrentRow.Cells[5].Value);

			try
			{
				// Para obtener la imagen.
				byte[] arrayImage = (byte[])_dataGridView1.CurrentRow.Cells[9].Value;
				//_imagePictureBox.Image = Objects.uploadimage.byteArrayToImage(arrayImage);
			}
			catch (Exception)
			{
				_imagePictureBox.Image = _imagBitmap;
			}
						
			_checkBoxCredito.Checked = Convert.ToBoolean(_dataGridView1.CurrentRow.Cells[8].Value);
			_checkBoxCredito.ForeColor = _checkBoxCredito.Checked ? Color.Green : Color.Red;
		}

		public void restablecer()
		{
			_accion = "insert";
			_num_pagina = 1;
			_imagePictureBox.Image = _imagBitmap;
			_textBoxCliente[0].Text = "";
			_textBoxCliente[1].Text = "";
			_textBoxCliente[2].Text = "";
			_textBoxCliente[3].Text = "";
			_textBoxCliente[4].Text = "";
			_textBoxCliente[5].Text = "";
			_checkBoxCredito.Checked = false;
			_checkBoxCredito.ForeColor = Color.LightSlateGray;
			_labelCliente[0].Text = "Nid";
			_labelCliente[0].ForeColor = Color.LightSlateGray;
			_labelCliente[1].Text = "Nombre";
			_labelCliente[1].ForeColor = Color.LightSlateGray;
			_labelCliente[2].Text = "Apellido";
			_labelCliente[2].ForeColor = Color.LightSlateGray;
			_labelCliente[3].Text = "Email";
			_labelCliente[3].ForeColor = Color.LightSlateGray;
			_labelCliente[4].Text = "Telefono";
			_labelCliente[4].ForeColor = Color.LightSlateGray;
			_labelCliente[5].Text = "Direccion";
			_labelCliente[5].ForeColor = Color.LightSlateGray;
			_ = SearchClientesAsync("");
			
		}
	}
}
